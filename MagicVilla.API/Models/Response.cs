using System.Net;

namespace MagicVilla.API.Models
{
	public class Response
	{
		public Response()
		{
			ErrorMessages = new List<string>();
		}
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; }
		public List<string>? ErrorMessages { get; set; }
		public object? Data { get; set; }
	}
}