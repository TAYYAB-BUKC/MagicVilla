using MagicVilla.API.Data;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
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
		public UserRepository(ApplicationDbContext dbContext, IConfiguration configuration)
		{
			_dbContext = dbContext;
			SecretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
		}

		public async Task<bool> IsUserUnique(string username)
		{
			var user = await _dbContext.LocalUsers.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
			if(user is null)
			{
				return true;
			}
			return false;
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO)
		{
			var user = await _dbContext.LocalUsers.FirstOrDefaultAsync(u => u.Username.ToLower() == requestDTO.Username.ToLower() && u.Password == requestDTO.Password);
			if (user is null) 
			{
				return new LoginResponseDTO()
				{
					Token = String.Empty,
					User = null
				};
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(SecretKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name , user.Username),
					new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role)
				}),
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

		public async Task<LocalUser> Regitser(RegistrationRequestDTO requestDTO)
		{
			LocalUser user = new LocalUser()
			{
				Name = requestDTO.Name,
				Username = requestDTO.Username,
				Password = requestDTO.Password,
				Role = requestDTO.Role,
			};

			await _dbContext.LocalUsers.AddAsync(user);
			await _dbContext.SaveChangesAsync();
			return user;
		}
	}
}
