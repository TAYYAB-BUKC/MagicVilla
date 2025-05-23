using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;

namespace MagicVilla.API.Data
{
	public static class VillaStore
	{
		public static List<VillaDTO> VillaList = new List<VillaDTO>()
		{
			new VillaDTO { Id = 1, Name = "Pool Villa", Occupancy = 100, SqFt = 10000 },
			new VillaDTO { Id = 2, Name = "Pool Villa", Occupancy = 250, SqFt = 20000 }
		};

		public static VillaDTO? GetVilla(int id)
		{
			return VillaStore.VillaList.Find(v => v.Id == id);
		}

		public static VillaDTO? CheckExistingVillaByName(string name)
		{
			return VillaStore.VillaList.FirstOrDefault(v => v?.Name?.ToLower() == name.ToLower());
		}
	}
}