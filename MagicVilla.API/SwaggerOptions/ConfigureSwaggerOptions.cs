using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MagicVilla.API.SwaggerOptions
{
	public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
	{
		public void Configure(SwaggerGenOptions options)
		{
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
			{
				Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
					  "Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
					  "Example: \"Bearer 123456abcdef\"",
				Scheme = "Bearer",
				In = ParameterLocation.Header,
				Name = "Authorization",
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference()
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header
					},
					new List<string>()
				}
			});

			options.SwaggerDoc("v1", new OpenApiInfo()
			{
				Version = "v1.0",
				Title = "Magic Villa v1.0",
				Description = "API version 1.0 to manage villas",
				TermsOfService = new Uri("https://example.com/terms"),
				Contact = new OpenApiContact()
				{
					Name = "Tayyab Arsalan",
					Email = "write2tayyabarsalan+linkedin@gmail.com",
					Url = new Uri("https://example.com/tayyabarsalan")
				},
				License = new OpenApiLicense()
				{
					Name = "Example License",
					Url = new Uri("https://example.com/license")
				}
			});

			options.SwaggerDoc("v2", new OpenApiInfo()
			{
				Version = "v2.0",
				Title = "Magic Villa v2.0",
				Description = "API version 2.0 to manage villas",
				TermsOfService = new Uri("https://example.com/terms"),
				Contact = new OpenApiContact()
				{
					Name = "Tayyab Arsalan",
					Email = "write2tayyabarsalan+linkedin@gmail.com",
					Url = new Uri("https://example.com/tayyabarsalan")
				},
				License = new OpenApiLicense()
				{
					Name = "Example License",
					Url = new Uri("https://example.com/license")
				}
			});
		}
	}
}
