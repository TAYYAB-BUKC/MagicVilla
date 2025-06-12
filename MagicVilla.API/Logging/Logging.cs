namespace MagicVilla.API.Logging
{
	public class Logging : ILogging
	{
		public void Log(string message, string type)
		{
			Console.WriteLine($"{type.ToUpper()} - {message}");
		}
	}
}