using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;

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
				return RedirectToAction(nameof(Index), nameof(HomeController));
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

		public IActionResult Logout()
		{
			return View();
		}

		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}