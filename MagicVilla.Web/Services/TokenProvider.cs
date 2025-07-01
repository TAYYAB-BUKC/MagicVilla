using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.Web.Services
{
	public class TokenProvider : ITokenProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public TokenProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public LoginResponseDTO GetToken()
		{
			return new LoginResponseDTO()
			{
				AccessToken = _httpContextAccessor.HttpContext?.Session.GetString(AccessToken),
				RefreshToken = _httpContextAccessor.HttpContext?.Session.GetString(RefreshToken),
			};
		}

		public void ClearToken()
		{
			_httpContextAccessor.HttpContext?.Session.SetString(AccessToken, string.Empty);
			_httpContextAccessor.HttpContext?.Session.SetString(RefreshToken, string.Empty);
		}

		public void SetToken(LoginResponseDTO model)
		{
			_httpContextAccessor.HttpContext?.Session.SetString(AccessToken, model.AccessToken);
			_httpContextAccessor.HttpContext?.Session.SetString(RefreshToken, model.RefreshToken);
		}
	}
}