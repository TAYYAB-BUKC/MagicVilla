using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class VillaNumberService : BaseService, IVillaNumberService
	{
		public IHttpClientFactory httpclient { get; set; }
		public string? BASE_URL { get; set; }
		public VillaNumberService(IHttpClientFactory httpclient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) : base(httpclient, httpContextAccessor)
		{
			this.httpclient = httpclient;
			this.BASE_URL = configuration.GetValue<string>("ServiceURLs:VillaAPI");
		}

		public async Task<T> GetAllAsync<T>()
		{
			return await SendAsync<T>(new Request()
			{
				RequestType = RequestType.GET,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI"
			});
		}

		public async Task<T> GetAsync<T>(int id)
		{
			return await SendAsync<T>(new Request()
			{
				RequestType = RequestType.GET,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI/{id}"
			});
		}

		public async Task<T> CreateAsync<T>(VillaNumberCreateDTO villa)
		{
			return await SendAsync<T>(new Request()
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI",
				Data = villa
			});
		}

		public async Task<T> UpdateAsync<T>(VillaNumberUpdateDTO villa)
		{
			return await SendAsync<T>(new Request()
			{
				RequestType = RequestType.PUT,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI/{villa.VillaNo}",
				Data = villa
			});
		}

		public async Task<T> DeleteAsync<T>(int id)
		{
			return await SendAsync<T>(new Request()
			{
				RequestType = RequestType.DELETE,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI/{id}",
			});
		}
	}
}