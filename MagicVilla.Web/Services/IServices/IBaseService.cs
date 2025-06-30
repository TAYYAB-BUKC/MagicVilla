using MagicVilla.Web.Models;

namespace MagicVilla.Web.Services.IServices
{
	public interface IBaseService
	{
		Response Response { get; set; }
		Task<T> SendAsync<T>(Request request, bool withBearer = true);
	}
}