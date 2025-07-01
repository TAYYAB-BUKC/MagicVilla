namespace MagicVilla.API.Models
{
	public class RefreshToken
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string JWTTokenId { get; set; }
		public string? JWTRefreshToken { get; set; }
		public bool IsValid { get; set; }
		public DateTime ExpiresAt { get; set; }
	}
}