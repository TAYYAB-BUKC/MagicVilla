using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla.Web.CustomException
{
	public class AuthExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			if(context.Exception is AuthException)
			{
				context.Result = new RedirectToActionResult("Login", "Auth", null);
			}
		}
	}
}