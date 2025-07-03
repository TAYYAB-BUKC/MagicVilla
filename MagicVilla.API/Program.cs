using MagicVilla.API.Data;
using MagicVilla.API.Logging;
using MagicVilla.API.Mappings;
using MagicVilla.API.Models;
using MagicVilla.API.Repository;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using static MagicVilla.Utility.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("log/logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers(options =>
{
    //options.ReturnHttpNotAcceptable = true;
    options.CacheProfiles.Add(CacheProfileName, new CacheProfile()
    {
        Duration = CacheDuration
    });
}).AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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
});

builder.Services.AddSingleton<ILogging, Logging>();
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddAutoMapper(typeof(MappingConfiguration));

var key = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = true,
        ValidIssuer = "https://localhost:7001",
        ValidateAudience = true,
        ValidAudience = "https://localhost:7002",
        ValidAudiences = new List<string> { "https://localhost:7002" },
		ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddApiVersioning(options => {
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddResponseCaching();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v2/swagger.json", "Magic_Villa_V2");
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_Villa_V1");
	});
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
