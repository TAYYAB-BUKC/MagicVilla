namespace MagicVilla.Web.Models.DTOs
{
	public class UserDTO
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
		public required string Role { get; set; }
	}
}