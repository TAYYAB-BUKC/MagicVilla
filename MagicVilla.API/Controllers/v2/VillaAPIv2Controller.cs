using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.API.Controllers.v2
{
	[Route("api/v{version:apiVersion}/VillaAPI")]
	[ApiController]
	[ApiVersion("2.0")]
	public class VillaAPIv2Controller : ControllerBase
	{
		[HttpGet("GetString")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IEnumerable<string> GetVillasVersion2()
		{
			return new List<string>()
			{
				"MagicString1",
				"MagicString2"
			};
		}
	}
}