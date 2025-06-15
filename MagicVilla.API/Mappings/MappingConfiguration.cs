using AutoMapper;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;

namespace MagicVilla.API.Mappings
{
	public class MappingConfiguration : Profile
	{
		public MappingConfiguration()
		{
			CreateMap<VillaDTO, Villa>().ReverseMap();
			CreateMap<VillaDTO, VillaCreateDTO>().ReverseMap();
			CreateMap<VillaDTO, VillaUpdateDTO>().ReverseMap();

			CreateMap<Villa, VillaCreateDTO>().ReverseMap();
			CreateMap<Villa, VillaUpdateDTO>().ReverseMap();
		}
	}
}
