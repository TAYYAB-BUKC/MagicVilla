namespace MagicVilla.Web.Models.DTOs
{
	public class LoginResponseDTO
	{
		public required UserDTO User { get; set; }
		public required string Token { get; set; }
	}
}