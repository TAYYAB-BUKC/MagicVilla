using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using System.Net.Http;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class VillaNumberService : IVillaNumberService
	{
		private readonly IHttpClientFactory httpclient;
		private readonly IBaseService _baseService;
		public string? BASE_URL { get; set; }
		public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService baseService)
		{
			this.httpclient = httpclient;
			this.BASE_URL = configuration.GetValue<string>("ServiceURLs:VillaAPI");
			this._baseService = baseService;
		}

		public async Task<T> GetAllAsync<T>()
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.GET,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI"
			});
		}

		public async Task<T> GetAsync<T>(int id)
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.GET,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI/{id}"
			});
		}

		public async Task<T> CreateAsync<T>(VillaNumberCreateDTO villa)
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI",
				Data = villa
			});
		}

		public async Task<T> UpdateAsync<T>(VillaNumberUpdateDTO villa)
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.PUT,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI/{villa.VillaNo}",
				Data = villa
			});
		}

		public async Task<T> DeleteAsync<T>(int id)
		{
			return await _baseService.SendAsync<T>(new Request()
			{
				RequestType = RequestType.DELETE,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaNumberAPI/{id}",
			});
		}
	}
}