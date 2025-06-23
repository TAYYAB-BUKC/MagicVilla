namespace MagicVilla.API.Models.DTOs
{
	public class LoginResponseDTO
	{
		public required ApplicationUser User { get; set; }
		public required string Token { get; set; }
	}
}