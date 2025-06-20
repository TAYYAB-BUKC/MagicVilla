namespace MagicVilla.API.Models.DTOs
{
	public class RegistrationRequestDTO
	{
		public required string Name { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
		public required string Role { get; set; }
	}
}