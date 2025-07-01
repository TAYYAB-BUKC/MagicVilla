namespace MagicVilla.Utility
{
	public static class Configuration
	{
		public enum RequestType
		{
			GET,
			POST,
			PUT,
			PATCH,
			DELETE
		}

		public static readonly string AccessToken = "SecureAccessToken";
		public static readonly string RefreshToken = "SecureRefreshToken"; 
		public static readonly string SessionUserId = "LoggedInUserID";
		public static readonly string SessionUserName = "LoggedInUserName";

		public static readonly string ApiVersion = "v2";

		public const string CacheProfileName = "Default30";
		public static readonly int CacheDuration = 30;

		public const string Role_Admin = "admin";
		public const string Role_User = "user";

		public enum ContentType
		{
			Json,
			MultipartFormData
		}
	}
}