using AutoMapper;
using MagicVilla.Web.Models.DTOs;

namespace MagicVilla.Web.Mappings
{
	public class MappingConfiguration : Profile
	{
		public MappingConfiguration()
		{
			CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
			CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();
			
			CreateMap<VillaNumberDTO, VillaNumberCreateDTO>().ReverseMap();
			CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();
		}
	}
}