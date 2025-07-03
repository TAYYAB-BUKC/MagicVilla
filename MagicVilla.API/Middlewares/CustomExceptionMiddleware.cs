using Newtonsoft.Json;

namespace MagicVilla.API.Middlewares
{
	public class CustomExceptionMiddleware
	{
		private RequestDelegate _requestDelegate;

		public CustomExceptionMiddleware(RequestDelegate requestDelegate)
		{
			_requestDelegate = requestDelegate;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _requestDelegate(context);
			}
			catch (Exception exception)
			{
				await ProcessError(context, exception);
			}
		}

		private async Task ProcessError(HttpContext context, Exception exception)
		{
			context.Response.StatusCode = 500;
			context.Response.ContentType = "application/json";
			if (exception is BadImageFormatException)
			{
				await context.Response.WriteAsync(JsonConvert.SerializeObject(new
				{
					From = "Program.cs",
					Title = "BadImageException",
					StatusCode = 550,
					ErrorMessage = exception.Message,
					StackTrace = exception.StackTrace
				}));
			}
			else
			{
				await context.Response.WriteAsync(JsonConvert.SerializeObject(new
				{
					From = "Program.cs",
					StatusCode = 500,
					ErrorMessage = exception.Message,
					StackTrace = exception.StackTrace,
				}));
			}
		}
	}
}