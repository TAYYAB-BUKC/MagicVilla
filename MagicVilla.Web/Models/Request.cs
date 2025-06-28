using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Models
{
	public class Request
	{
		public RequestType RequestType { get; set; }
		public required string URL { get; set; }
		public object? Data { get; set; }
		public ContentType ContentType { get; set; } = ContentType.Json;
	}
}