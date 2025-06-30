using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		public IActionResult Login()
		{
			LoginRequestDTO model = new();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequestDTO request)
		{
			var response = await _authService.LoginAsync<Response>(request);
			if(response is not null && response.IsSuccess)
			{
				var model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Data));
				
				var tokenHandler = new JwtSecurityTokenHandler();
				var token = tokenHandler.ReadJwtToken(model.AccessToken);

				HttpContext.Session.SetString(AccessToken, model.AccessToken);
				HttpContext.Session.SetString(SessionUserId, token.Claims.FirstOrDefault(c => c.Type == "nameidentifier").Value);
				HttpContext.Session.SetString(SessionUserName, token.Claims.FirstOrDefault(c => c.Type == "name").Value);

				var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
				identity.AddClaim(new Claim(ClaimTypes.Name, token.Claims.FirstOrDefault(c => c.Type == "name").Value));
				identity.AddClaim(new Claim(ClaimTypes.Role, token.Claims.FirstOrDefault(c => c.Type == "role").Value));

				var principal = new ClaimsPrincipal(identity);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

				return RedirectToAction("Index", "Home");
			}
			foreach (var errorMessage in response.ErrorMessages)
			{
				ModelState.AddModelError("CustomError", errorMessage);
			}
			return View(request);
		}

		public IActionResult Register()
		{
			RegistrationRequestDTO model = new();
			List<SelectListItem> list = new () 
			{
				new SelectListItem () { Text = Role_Admin, Value = Role_Admin },
				new SelectListItem () { Text = Role_User, Value = Role_User }
			};
			ViewBag.Roles = list;
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegistrationRequestDTO request)
		{
			if (string.IsNullOrWhiteSpace(request.Role))
			{
				request.Role = Role_User;
			}
			var response = await _authService.RegisterAsync<Response>(request);
			if (response is not null && !response.IsSuccess)
			{
				return RedirectToAction(nameof(Login));
			}
			return View(request);
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			HttpContext.Session.SetString(AccessToken, string.Empty);
			HttpContext.Session.SetString(SessionUserId, string.Empty);
			HttpContext.Session.SetString(SessionUserName, string.Empty);
			return RedirectToAction("Index", "Home");
		}

		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}