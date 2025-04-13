using MagicVilla.API.Models;

namespace MagicVilla.API.Data
{
	public static class VillaStore
	{
		public static List<Villa> VillaList = new List<Villa>()
		{
			new Villa { Id = 1, Name = "Pool Villa" },
			new Villa { Id = 2, Name = "Pool Villa" }
		};
	}
}