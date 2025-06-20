using MagicVilla.API.Data;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.API.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _dbContext;
		public UserRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
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

			return new LoginResponseDTO()
			{
				Token = String.Empty, // Generate Token Using JWT
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
