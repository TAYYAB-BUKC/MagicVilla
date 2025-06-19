using AutoMapper;
using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla.Web.Controllers
{
	public class VillaController : Controller
	{
		public IVillaService _villaService { get; set; }
		public IMapper _mapper { get; set; }

		public VillaController(IVillaService villaService, IMapper mapper)
		{
			_villaService = villaService;
			_mapper = mapper;			
		}

		public async Task<IActionResult> IndexVilla()
		{
			List<VillaDTO> list = new();

			var response = await _villaService.GetAllAsync<Response>();
			if(response is not null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Data));
			}
			return View(list);
		}
	}
}
