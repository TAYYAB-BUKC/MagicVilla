using MagicVilla.Web.Models;

namespace MagicVilla.Web.Services.IServices
{
	public interface IApiMessageRequestBuilder
	{
		HttpRequestMessage Build(Request request);
	}
}