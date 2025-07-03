using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla.API.ExceptionFilters
{
	public class CustomExceptionFilter : IActionFilter
	{
		public void OnActionExecuted(ActionExecutedContext context)
		{
			if(context.Exception is FileNotFoundException fileNotFoundException)
			{
				context.Result = new ObjectResult($"File not found exception handle by {nameof(CustomExceptionFilter)}")
				{
					StatusCode = 500
				};
				context.ExceptionHandled = true;
			}
		}

		public void OnActionExecuting(ActionExecutingContext context)
		{

		}
	}
}
