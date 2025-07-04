﻿using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class VillaService : IVillaService
	{
		private readonly IHttpClientFactory httpClient;
		private readonly IBaseService _baseService;
		public string? BASE_URL { get; set; }

		public VillaService(IHttpClientFactory httpClient, IConfiguration configuration, IBaseService baseService)
		{
			this.httpClient = httpClient;
			this.BASE_URL = configuration.GetValue<string>("ServiceURLs:VillaAPI");
			this._baseService = baseService;
		}

		public async Task<T> GetAllAsync<T>()
		{
			return await _baseService.SendAsync<T>(new Request
			{
				RequestType = RequestType.GET,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaAPI"
			});
		}

		public async Task<T> GetAsync<T>(int id)
		{
			return await _baseService.SendAsync<T>(new Request
			{
				RequestType = RequestType.GET,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaAPI/{id}"
			});
		}

		public async Task<T> CreateAsync<T>(VillaCreateDTO villa)
		{
			return await _baseService.SendAsync<T>(new Request
			{
				RequestType = RequestType.POST,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaAPI",
				Data = villa,
				ContentType = ContentType.MultipartFormData
			});
		}

		public async Task<T> UpdateAsync<T>(VillaUpdateDTO villa)
		{
			return await _baseService.SendAsync<T>(new Request
			{
				RequestType = RequestType.PUT,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaAPI/{villa.Id}",
				Data = villa,
				ContentType = ContentType.MultipartFormData
			});
		}

		public async Task<T> DeleteAsync<T>(int id)
		{
			return await _baseService.SendAsync<T>(new Request
			{
				RequestType = RequestType.DELETE,
				URL = $"{BASE_URL}/api/{ApiVersion}/VillaAPI/{id}"
			});
		}
	}
}