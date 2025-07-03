using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MagicVilla.API.SwaggerOptions
{
	public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;
		public ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
		{
			_apiVersionDescriptionProvider = apiVersionDescriptionProvider;
		}

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

			foreach (var version in _apiVersionDescriptionProvider.ApiVersionDescriptions)
			{
				options.SwaggerDoc(version.GroupName, new OpenApiInfo()
				{
					Version = $"v{version.ApiVersion.ToString()}",
					Title = $"Magic Villa v{version.ApiVersion.ToString()}",
					Description = $"API version {version.ApiVersion.ToString()} to manage villas",
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
}
