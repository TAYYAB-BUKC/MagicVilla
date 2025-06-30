using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using System.Net.Http;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class AuthService : BaseService, IAuthService
	{
		public IHttpClientFactory httpclient { get; set; }
		public string? BASE_URL { get; set; }
		public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, ITokenProvider provider) : base(httpClient, provider)
		{
			this.httpclient = httpClient;
			this.BASE_URL = configuration.GetValue<string>("ServiceURLs:VillaAPI");
		}

		public async Task<T> LoginAsync<T>(LoginRequestDTO requestDTO)
		{
			return await SendAsync<T>(new Request()
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/UserAuth/login",
				Data = requestDTO
			});
		}

		public async Task<T> RegisterAsync<T>(RegistrationRequestDTO requestDTO)
		{
			return await SendAsync<T>(new Request()
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/UserAuth/register",
				Data = requestDTO
			});
		}
	}
}