using MagicVilla.Web.Models.DTOs;

namespace MagicVilla.Web.Services.IServices
{
	public interface ITokenProvider
	{
		LoginResponseDTO GetToken();
		void SetToken(LoginResponseDTO model);
		void ClearToken();
	}
}