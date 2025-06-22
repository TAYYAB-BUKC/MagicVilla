using MagicVilla.Web.Models.DTOs;

namespace MagicVilla.Web.Services.IServices
{
	public interface IAuthService
	{
		Task<T> LoginAsync<T>(LoginRequestDTO requestDTO);
		Task<T> RegisterAsync<T>(RegistrationRequestDTO requestDTO);
	}
}