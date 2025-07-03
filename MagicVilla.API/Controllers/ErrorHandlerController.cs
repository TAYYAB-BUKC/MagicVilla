using Microsoft.AspNetCore.Diagnostics;
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
		public IActionResult ProcessError([FromServices] IHostEnvironment hostEnvironment, [FromServices] IHttpContextAccessor httpContextAccessor)
		{
			if (hostEnvironment.IsDevelopment())
			{
				var feature = httpContextAccessor?.HttpContext?.Features.Get<IExceptionHandlerFeature>();
				return Problem(
					detail: feature.Error.StackTrace,
					title: feature.Error.Message,
					instance: hostEnvironment.EnvironmentName
					);
			}
			else
			{
				return Problem();
			}
		}
	}
}