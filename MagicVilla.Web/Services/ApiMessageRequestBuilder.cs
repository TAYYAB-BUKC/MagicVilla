using MagicVilla.Web.Models;
using MagicVilla.Web.Services.IServices;
using Newtonsoft.Json;
using System.Text;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class ApiMessageRequestBuilder : IApiMessageRequestBuilder
	{
		public HttpRequestMessage Build(Request request)
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
		}
	}
}