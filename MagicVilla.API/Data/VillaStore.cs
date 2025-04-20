using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;

namespace MagicVilla.API.Data
{
	public static class VillaStore
	{
		public static List<VillaDTO> VillaList = new List<VillaDTO>()
		{
			new VillaDTO { Id = 1, Name = "Pool Villa" },
			new VillaDTO { Id = 2, Name = "Pool Villa" }
		};
	}
}