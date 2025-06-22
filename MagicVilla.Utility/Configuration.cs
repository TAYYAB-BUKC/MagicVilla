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

		public static readonly string SessionToken = "SecureToken";
		public static readonly string SessionUserId = "LoggedInUserID";
		public static readonly string SessionUserName = "LoggedInUserName";
	}
}
