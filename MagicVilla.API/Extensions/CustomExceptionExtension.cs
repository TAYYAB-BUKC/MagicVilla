using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;

namespace MagicVilla.API.Extensions
{
	public static class CustomExceptionExtension
	{
		public static void UseCustomExceptionHandler(this IApplicationBuilder app, bool IsDevelopmentEnvironment)
		{
			app.UseExceptionHandler(error =>
			{
				error.Run(async context =>
				{
					context.Response.StatusCode = 500;
					context.Response.ContentType = "application/json";
					var feature = context.Features.Get<IExceptionHandlerFeature>();
					if (feature is not null)
					{
						if (IsDevelopmentEnvironment)
						{
							if (feature.Error is BadImageFormatException)
							{
								await context.Response.WriteAsync(JsonConvert.SerializeObject(new
								{
									From = "Program.cs",
									Title = "BadImageException",
									StatusCode = 550,
									ErrorMessage = feature.Error.Message,
									StackTrace = feature.Error.StackTrace
								}));
							}
							else
							{
								await context.Response.WriteAsync(JsonConvert.SerializeObject(new
								{
									From = "Program.cs",
									StatusCode = 500,
									ErrorMessage = feature.Error.Message,
									StackTrace = feature.Error.StackTrace,
								}));
							}
						}
						else
						{
							await context.Response.WriteAsync(JsonConvert.SerializeObject(new
							{
								StatusCode = 500,
								Title = feature.Error.Message,
								Details = feature.Error.StackTrace,
							}));
						}
					}
				});
			});
		}
	}
}