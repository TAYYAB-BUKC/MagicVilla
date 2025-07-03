using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.API.Controllers
{
	[Route("api/ErrorHandler")]
	[ApiController]
	[ApiVersionNeutral]
	public class ErrorHandlerController : ControllerBase
	{
		[Route("ProcessError")]
		public IActionResult ProcessError()
		{
			return Problem();
		}
	}
}