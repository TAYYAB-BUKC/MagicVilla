namespace MagicVilla.Web.Models
{
	public class LocalUser
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }
		public required string Role { get; set; }
	}
}