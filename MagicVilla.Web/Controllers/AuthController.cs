using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
			if(response is not null && !response.IsSuccess)
			{
				var model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Data));
				HttpContext.Session.SetString(SessionToken, model.Token);
				HttpContext.Session.SetString(SessionUserId, Convert.ToString(model.User.Id));
				HttpContext.Session.SetString(SessionUserName, model.User.Name);
				return RedirectToAction(nameof(Index), nameof(HomeController));
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
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Regitser(RegistrationRequestDTO request)
		{
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
			HttpContext.Session.SetString(SessionToken, string.Empty);
			HttpContext.Session.SetString(SessionUserId, string.Empty);
			HttpContext.Session.SetString(SessionUserName, string.Empty);
			return RedirectToAction(nameof(Index), nameof(HomeController));
		}

		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}