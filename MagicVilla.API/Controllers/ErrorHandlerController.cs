using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.API.Controllers
{
	[Route("api/ErrorHandler")]
	[ApiController]
	[ApiVersionNeutral]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class ErrorHandlerController : ControllerBase
	{
		[Route("ProcessError")]
		public IActionResult ProcessError()
		{
			return Problem();
		}
	}
}