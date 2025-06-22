using MagicVilla.Web.Models;
using MagicVilla.Web.Services.IServices;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class BaseService : IBaseService
	{
		public Response Response { get; set; }
		private readonly IHttpClientFactory _httpClient;
		private readonly IHttpContextAccessor _httpContextAccessor;
		public BaseService(IHttpClientFactory httpClient, IHttpContextAccessor httpContextAccessor)
		{
			Response = new();
			_httpClient = httpClient;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<T> SendAsync<T>(Request request)
		{
			try
			{
				var client = _httpClient.CreateClient("MagicAPI");
				HttpRequestMessage httpRequest = new HttpRequestMessage();
				httpRequest.Headers.Add("Accept", "application/json");
				httpRequest.RequestUri = new Uri(request.URL);
				if(request.Data is not null)
				{
					httpRequest.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
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

				if (_httpContextAccessor.HttpContext is not null && !string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext.Session.GetString(SessionToken)))
				{
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpContextAccessor.HttpContext.Session.GetString(SessionToken));
				}

				var apiResponse = await client.SendAsync(httpRequest);
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
	}
}