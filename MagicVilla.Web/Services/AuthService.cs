using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class AuthService : BaseService, IAuthService
	{
		public IHttpClientFactory httpclient { get; set; }
		public string? BASE_URL { get; set; }
		public AuthService(IHttpClientFactory httpclient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpclient, httpContextAccessor)
		{
			this.httpclient = httpclient;
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