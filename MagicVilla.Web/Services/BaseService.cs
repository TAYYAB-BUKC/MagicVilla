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

		public BaseService(IHttpClientFactory httpClient, ITokenProvider tokenProvider, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		{
			Response = new();
			_httpClient = httpClient;
			_tokenProvider = tokenProvider;
			BASE_URL = configuration.GetValue<string>("ServiceURLs:VillaAPI");
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<T> SendAsync<T>(Request request, bool withBearer = true)
		{
			try
			{
				var client = _httpClient.CreateClient("MagicAPI");
				var httpRequestMessageFactory = () =>
				{
					HttpRequestMessage httpRequest = new();
					if (request.ContentType == ContentType.MultipartFormData)
					{
						httpRequest.Headers.Add("Accept", "*/*");
					}
					else
					{
						httpRequest.Headers.Add("Accept", "application/json");
					}
					httpRequest.RequestUri = new Uri(request.URL);

					if (request.ContentType == ContentType.MultipartFormData)
					{
						var content = new MultipartFormDataContent();

						foreach (var prop in request.Data.GetType().GetProperties())
						{
							var value = prop.GetValue(request.Data);
							if (value is FormFile)
							{
								var file = (FormFile)value;
								if (file is not null)
								{
									content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
								}
							}
							else
							{
								content.Add(new StringContent(Convert.ToString(value)), prop.Name);
							}
						}

						httpRequest.Content = content;
					}
					else
					{
						if (request.Data is not null)
						{
							httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
						}
					}

					switch (request.RequestType)
					{
						case RequestType.GET:
							httpRequest.Method = HttpMethod.Get;
							break;
						case RequestType.POST:
							httpRequest.Method = HttpMethod.Post;
							break;
						case RequestType.PUT:
							httpRequest.Method = HttpMethod.Put;
							break;
						case RequestType.PATCH:
							httpRequest.Method = HttpMethod.Patch;
							break;
						case RequestType.DELETE:
							httpRequest.Method = HttpMethod.Delete;
							break;
					}

					return httpRequest;
				};

				var apiResponse = await SendWithRefreshTokenAsync(client, httpRequestMessageFactory, withBearer);
				var apiContent = await apiResponse.Content.ReadAsStringAsync();

				try
				{
					Response response = JsonConvert.DeserializeObject<Response>(apiContent);
					if(apiResponse.StatusCode == HttpStatusCode.BadRequest)
					{
						response.StatusCode = HttpStatusCode.BadRequest;
						response.IsSuccess = false;
					}

					if (apiResponse.StatusCode == HttpStatusCode.NotFound)
					{
						response.StatusCode = HttpStatusCode.NotFound;
						response.IsSuccess = false;
					}

					var serializeResponse = JsonConvert.SerializeObject(response);
					var finalReponse = JsonConvert.DeserializeObject<T>(serializeResponse);
					return finalReponse;
				}
				catch (Exception ex)
				{
					var response = JsonConvert.DeserializeObject<T>(apiContent);
					return response;
				}
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
				if (apiResponse.IsSuccessStatusCode)
				{
					return apiResponse;
				}

				// Check whether the RefreshToken is still valid or not 
				await InvokeRefreshTokenEndpointAsync(httpClient, token);
				return await httpClient.SendAsync(httpRequestMessage());
			}
			catch (HttpRequestException httpRequestException)
			{
				if(httpRequestException.StatusCode == HttpStatusCode.Unauthorized)
				{
					await InvokeRefreshTokenEndpointAsync(httpClient, token);
					return await httpClient.SendAsync(httpRequestMessage());
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