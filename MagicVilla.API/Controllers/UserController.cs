using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla.API.Controllers
{
	[Route("api/v{version:apiVersion}/UserAuth")]
	[ApiController]
	[ApiVersionNeutral]
	public class UserController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly Response _response;

		public UserController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
			_response = new();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO requestDTO)
		{
			var response = await _userRepository.Login(requestDTO);
			if (response is null || string.IsNullOrWhiteSpace(response.AccessToken))
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("user not found");
				return BadRequest(_response);
			}

			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Data = response;
			return Ok(_response);
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO requestDTO)
		{
			var isUserUnique = await _userRepository.IsUserUnique(requestDTO.Username);
			if (!isUserUnique)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("username already exists");
				return BadRequest(_response);
			}

			var response = await _userRepository.Regitser(requestDTO);
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);
		}

		[HttpPost("refresh")]
		public async Task<IActionResult> GenerateNewToken([FromBody] LoginResponseDTO requestDTO)
		{
			if (ModelState.IsValid)
			{
				var response = await _userRepository.GenerateTokens(requestDTO);
				if (response is null || string.IsNullOrWhiteSpace(response.AccessToken))
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					_response.ErrorMessages?.Add("Invalid token");
					return BadRequest(_response);
				}

				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				_response.Data = response;
				return Ok(_response);
			}

			_response.StatusCode = HttpStatusCode.BadRequest;
			_response.IsSuccess = false;
			_response.ErrorMessages?.Add("Invalid request");
			return BadRequest(_response);			
		}

		[HttpPost("revoke")]
		public async Task<IActionResult> RevokeRefreshToken([FromBody] LoginResponseDTO requestDTO)
		{
			if (ModelState.IsValid)
			{
				await _userRepository.RevokeRefreshToken(requestDTO);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);	
			}

			_response.StatusCode = HttpStatusCode.BadRequest;
			_response.IsSuccess = false;
			_response.ErrorMessages?.Add("Invalid token");
			return BadRequest(_response);
		}

		[HttpGet("error")]
		public IActionResult Error()
		{
			throw new FileNotFoundException();
		}
	}
}