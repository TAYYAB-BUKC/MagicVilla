using MagicVilla.API.Data;
using MagicVilla.API.ExceptionFilters;
using MagicVilla.API.Extensions;
using MagicVilla.API.Logging;
using MagicVilla.API.Mappings;
using MagicVilla.API.Models;
using MagicVilla.API.Repository;
using MagicVilla.API.Repository.Interfaces;
using MagicVilla.API.SwaggerOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using static MagicVilla.Utility.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
    options.Filters.Add<CustomExceptionFilter>();
}).AddNewtonsoftJson().ConfigureApiBehaviorOptions(options =>
{
    options.ClientErrorMapping[StatusCodes.Status500InternalServerError] = new ClientErrorData()
    {
        Link = "https://fake.com/500Error"
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
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

ApplyPendingMigrations();

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

//app.UseExceptionHandler("/api/ErrorHandler/ProcessError");

app.UseCustomExceptionHandler(app.Environment.IsDevelopment());

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


void ApplyPendingMigrations()
{
    using (var scope = app.Services.CreateScope())
    {
        var _dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if(_dbContext is null)
        {
            return;
        }

        if(_dbContext.Database.GetPendingMigrations().Count() > 0)
        {
            _dbContext.Database.Migrate();
        }
    }
}