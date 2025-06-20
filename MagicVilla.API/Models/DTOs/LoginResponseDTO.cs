namespace MagicVilla.API.Models.DTOs
{
	public class LoginResponseDTO
	{
		public required LocalUser User { get; set; }
		public required string Token { get; set; }
	}
}