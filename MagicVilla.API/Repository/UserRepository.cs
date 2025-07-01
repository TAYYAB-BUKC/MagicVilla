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
					AccessToken = string.Empty,
					RefreshToken = string.Empty
				};
			}

			var tokenID = $"JTI_{Guid.NewGuid().ToString("N")}";
			var accessToken = await GenerateAccessToken(user, tokenID);
			var refreshToken = await GenerateNewRefreshToken(user.Id, tokenID);

			return new LoginResponseDTO()
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken
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

		private async Task<string> GenerateAccessToken(ApplicationUser user, string tokenID)
		{
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
			claims.Add(new Claim(JwtRegisteredClaimNames.Jti, tokenID));
			claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddMinutes(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		public async Task<LoginResponseDTO> GenerateTokens(LoginResponseDTO requestDTO)
		{
			// Find an existing RefreshToken

			var existingRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.JWTRefreshToken == requestDTO.RefreshToken);

			if(existingRefreshToken is null)
			{
				return new LoginResponseDTO()
				{
					AccessToken = string.Empty,
					RefreshToken = string.Empty
				};
			}

			// Compare data from existing RefreshToken with the provided AccessToken and if there is a mismatch then consider it a fraud request

			var tokenDetails = GetAccessTokenData(requestDTO.AccessToken);

			if(!tokenDetails.IsSuccess
				|| existingRefreshToken.JWTTokenId != tokenDetails.TokenID
				|| existingRefreshToken.UserId != tokenDetails.userID)
			{
				existingRefreshToken.IsValid = false;
				await _dbContext.SaveChangesAsync();

				return new LoginResponseDTO()
				{
					AccessToken = string.Empty,
					RefreshToken = string.Empty
				};
			}

			// When someone tries to use invalid RefreshToken the it's a fraud request

			if (!existingRefreshToken.IsValid)
			{
				var tokenChain = await _dbContext.RefreshTokens.Where(rt => rt.UserId == existingRefreshToken.UserId
								&& rt.JWTTokenId == existingRefreshToken.JWTTokenId)
								.ExecuteUpdateAsync(rt =>rt.SetProperty(rt=>rt.IsValid, false));

				//tokenChain.ForEach(t => t.IsValid = false);

				//_dbContext.UpdateRange(tokenChain);
				await _dbContext.SaveChangesAsync();

				return new LoginResponseDTO()
				{
					AccessToken = string.Empty,
					RefreshToken = string.Empty
				};
			}

			// If RefreshToken has expired then mark it as invalid

			if (existingRefreshToken.ExpiresAt < DateTime.Now)
			{
				existingRefreshToken.IsValid = false;
				await _dbContext.SaveChangesAsync();

				return new LoginResponseDTO()
				{
					AccessToken = string.Empty,
					RefreshToken = string.Empty
				};
			}

			// Replace old RefreshToken with the new expire date

			var newRefreshToken = await GenerateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JWTTokenId);

			// Revoke existing RefreshToken
			if (!string.IsNullOrWhiteSpace(newRefreshToken))
			{
				existingRefreshToken.IsValid = false;
				await _dbContext.SaveChangesAsync();
			}

			// Generate New AccessToken
			var user = await _dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == existingRefreshToken.UserId);
			var newAccessToken = await GenerateAccessToken(user, existingRefreshToken.JWTTokenId);

			return new LoginResponseDTO()
			{
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken,
			};
		}

		private (bool IsSuccess, string userID, string TokenID) GetAccessTokenData(string AccessToken)
		{
			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var data = tokenHandler.ReadJwtToken(AccessToken);
				var userId = data.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
				var tokenId = data.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
				return (true, userId, tokenId);
			}
			catch (Exception e)
			{
				return (false, string.Empty, string.Empty);
			}
		}

		private async Task<string> GenerateNewRefreshToken(string userId, string tokenId)
		{
			RefreshToken refreshToken = new()
			{
				IsValid = true,
				JWTTokenId = tokenId,
				UserId = userId,
				JWTRefreshToken = $"{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}{Guid.NewGuid().ToString("N")}",
				ExpiresAt = DateTime.Now.AddMinutes(2),
			};

			await _dbContext.RefreshTokens.AddAsync(refreshToken);
			await _dbContext.SaveChangesAsync();

			return refreshToken.JWTRefreshToken;
		}
	}
}