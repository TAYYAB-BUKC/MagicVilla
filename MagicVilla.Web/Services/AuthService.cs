using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class AuthService : IAuthService
	{
		private readonly IHttpClientFactory httpclient;
		private readonly IBaseService _baseService;
		public string? BASE_URL { get; set; }
		public AuthService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService baseService)
		{
			this.httpclient = httpClient;
			this.BASE_URL = configuration.GetValue<string>("ServiceURLs:VillaAPI");
			this._baseService = baseService;
		}

		public async Task<T> LoginAsync<T>(LoginRequestDTO requestDTO)
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/UserAuth/login",
				Data = requestDTO
			}, false);
		}

		public async Task<T> RegisterAsync<T>(RegistrationRequestDTO requestDTO)
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/UserAuth/register",
				Data = requestDTO
			}, false);
		}

		public async Task<T> LogOutAsync<T>(LoginResponseDTO requestDTO)
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/UserAuth/revoke",
				Data = requestDTO
			});
		}
	}
}