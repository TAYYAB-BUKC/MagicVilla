using MagicVilla.API.Data;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla.API.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _dbContext;
		public string? SecretKey { get; set; }
		public readonly UserManager<ApplicationUser> _userManager;
		public readonly RoleManager<IdentityRole> _roleManager;
		public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_dbContext = dbContext;
			SecretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public async Task<bool> IsUserUnique(string username)
		{
			var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());
			if(user is null)
			{
				return true;
			}
			return false;
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO)
		{
			var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == requestDTO.Username.ToLower());
			var isValid = await _userManager.CheckPasswordAsync(user, requestDTO.Password);

			if (user is null || !isValid) 
			{
				return new LoginResponseDTO()
				{
					Token = String.Empty,
					User = null
				};
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(SecretKey);

			var roles = await _userManager.GetRolesAsync(user);

			List<Claim> claims = new List<Claim>();
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
					
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddMinutes(30),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return new LoginResponseDTO()
			{
				Token = tokenHandler.WriteToken(token),
				User = user
 			};
		}

		public async Task<ApplicationUser> Regitser(RegistrationRequestDTO requestDTO)
		{
			ApplicationUser user = new ApplicationUser()
			{
				Name = requestDTO.Name,
				Email = requestDTO.Username,
				NormalizedEmail = requestDTO.Username.ToUpper(),
				UserName = requestDTO.Username,
			};

			var response = await _userManager.CreateAsync(user, requestDTO.Password);
			if (response.Succeeded)
			{
				if (!await _roleManager.RoleExistsAsync(requestDTO.Role))
				{
					await _roleManager.CreateAsync(new IdentityRole(requestDTO.Role));
				}
				await _userManager.AddToRoleAsync(user, requestDTO.Role);
			}
			return user;
		}
	}
}
