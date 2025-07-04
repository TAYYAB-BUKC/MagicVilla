﻿using MagicVilla.Web.CustomException;
using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class BaseService : IBaseService
	{
		public Response Response { get; set; }
		private readonly IHttpClientFactory _httpClient;
		private readonly ITokenProvider _tokenProvider;
		private readonly string BASE_URL;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IApiMessageRequestBuilder _apiMessageRequestBuilder;

		public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IApiMessageRequestBuilder apiMessageRequestBuilder)
		{
			Response = new();
			_httpClient = httpClient;
			_tokenProvider = tokenProvider;
			BASE_URL = configuration.GetValue<string>("ServiceURLs:VillaAPI");
			_httpContextAccessor = httpContextAccessor;
			_apiMessageRequestBuilder = apiMessageRequestBuilder;
		}

		public async Task<T> SendAsync<T>(Request request, bool withBearer = true)
		{
			try
			{
				var client = _httpClient.CreateClient("MagicAPI");
				var httpRequestMessageFactory = () =>
				{
					return _apiMessageRequestBuilder.Build(request);
				};

				var apiResponse = await SendWithRefreshTokenAsync(client, httpRequestMessageFactory, withBearer);

				var finalResponse = new Response()
				{
					IsSuccess = false
				};

				try
				{
					var apiContent = await apiResponse.Content.ReadAsStringAsync();
					finalResponse = JsonConvert.DeserializeObject<Response>(apiContent);
					switch (apiResponse.StatusCode)
					{
						case HttpStatusCode.NotFound:
							finalResponse.StatusCode = HttpStatusCode.NotFound;
							finalResponse.ErrorMessages = new List<string> { "Not Found" };
							break;
						case HttpStatusCode.Forbidden:
							finalResponse.StatusCode = HttpStatusCode.Forbidden;
							finalResponse.ErrorMessages = new List<string> { "Access Denied" };
							break;
						case HttpStatusCode.Unauthorized:
							finalResponse.StatusCode = HttpStatusCode.Unauthorized;
							finalResponse.ErrorMessages = new List<string> { "Unauthorized" };
							break;
						case HttpStatusCode.InternalServerError:
							finalResponse.StatusCode = HttpStatusCode.InternalServerError;
							finalResponse.ErrorMessages = new List<string> { "Internal Server Error" };
							break;
						case HttpStatusCode.OK:
							finalResponse.StatusCode = HttpStatusCode.OK;
							finalResponse.IsSuccess = true;
							break;
						default:
							break;
					}
				}
				catch (Exception ex)
				{
					finalResponse.ErrorMessages = new List<string>() { "Error Encountered", Convert.ToString(ex.Message) };
				}

				var serializeResponse = JsonConvert.SerializeObject(finalResponse);
				var finalReponse = JsonConvert.DeserializeObject<T>(serializeResponse);
				return finalReponse;
			}
			catch (AuthException)
			{
				throw;
			}
			catch(Exception ex)
			{
				var response = new Response
				{
					ErrorMessages = new List<string>() { Convert.ToString(ex.Message) },
					IsSuccess = false,
					StatusCode = HttpStatusCode.InternalServerError
				};

				var serializeResponse = JsonConvert.SerializeObject(response);
				var finalReponse = JsonConvert.DeserializeObject<T>(serializeResponse);
				return finalReponse;
			}
		}

		private async Task<HttpResponseMessage> SendWithRefreshTokenAsync(HttpClient httpClient, Func<HttpRequestMessage> httpRequestMessage, bool withBearer)
		{
			var token = _tokenProvider.GetToken();
			if (withBearer && token is not null && !string.IsNullOrWhiteSpace(token.AccessToken))
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
			}

			try
			{
				var apiResponse = await httpClient.SendAsync(httpRequestMessage());

				// Check whether the RefreshToken is still valid or not 
				if (apiResponse.StatusCode == HttpStatusCode.Unauthorized)
				{
					await InvokeRefreshTokenEndpointAsync(httpClient, token);
					return await httpClient.SendAsync(httpRequestMessage());
				}

				return apiResponse;
			}
			catch (AuthException)
			{
				throw;
			}
			catch (HttpRequestException httpRequestException)
			{
				try
				{
					if (httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
					{
						await InvokeRefreshTokenEndpointAsync(httpClient, token);
						return await httpClient.SendAsync(httpRequestMessage());
					}
				}
				catch (AuthException)
				{
					throw;
				}
				throw;
			}
		}

		private async Task InvokeRefreshTokenEndpointAsync(HttpClient httpClient, LoginResponseDTO responseDTO)
		{
			HttpRequestMessage httpRequest = new HttpRequestMessage();
			httpRequest.Method = HttpMethod.Post;
			httpRequest.Headers.Add("accept", "application/json");
			httpRequest.RequestUri = new Uri($"{BASE_URL}/api/{ApiVersion}/userauth/refresh");
			httpRequest.Content = new StringContent(JsonConvert.SerializeObject(responseDTO), Encoding.UTF8, "application/json");

			try
			{
				var response = await httpClient.SendAsync(httpRequest);
				var responseContent = await response.Content.ReadAsStringAsync();
				var apiResponse = JsonConvert.DeserializeObject<Response>(responseContent);

				if (apiResponse is null || !apiResponse.IsSuccess)
				{
					await _httpContextAccessor.HttpContext.SignOutAsync();
					_tokenProvider.ClearToken();
					throw new AuthException();
				}

				var contentData = JsonConvert.SerializeObject(apiResponse.Data);
				var data = JsonConvert.DeserializeObject<LoginResponseDTO>(contentData);

				if(data is not null && !string.IsNullOrWhiteSpace(data.AccessToken))
				{
					httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.AccessToken);
					await SignInUserWithNewTokensAsync(data);
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		private async Task SignInUserWithNewTokensAsync(LoginResponseDTO responseDTO)
		{
			// Sign in user with new Tokens
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.ReadJwtToken(responseDTO.AccessToken);

			_tokenProvider.SetToken(responseDTO);
			_httpContextAccessor.HttpContext.Session.SetString(SessionUserId, token.Claims.FirstOrDefault(c => c.Type == "nameid").Value);
			_httpContextAccessor.HttpContext.Session.SetString(SessionUserName, token.Claims.FirstOrDefault(c => c.Type == "unique_name").Value);

			var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
			identity.AddClaim(new Claim(ClaimTypes.Name, token.Claims.FirstOrDefault(c => c.Type == "unique_name").Value));
			identity.AddClaim(new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(c => c.Type == "role").Value));

			var principal = new ClaimsPrincipal(identity);
			await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
		}
	}
}