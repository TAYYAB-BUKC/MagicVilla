using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;

namespace MagicVilla.API.Repository.Interfaces
{
	public interface IUserRepository
	{
		Task<bool> IsUserUnique(string username);
		Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO);
		Task<ApplicationUser> Regitser(RegistrationRequestDTO requestDTO);
		Task<LoginResponseDTO> GenerateTokens(LoginResponseDTO requestDTO);
	}
}