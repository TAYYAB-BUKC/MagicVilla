using AutoMapper;
using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla.Web.Controllers
{
	public class VillaNumberController : Controller
	{
		public IVillaNumberService _villaNumberService { get; set; }
		public IMapper _mapper { get; set; }

		public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper)
		{
			_villaNumberService = villaNumberService;
			_mapper = mapper;
		}

		public async Task<IActionResult> IndexVillaNumber()
		{
			List<VillaNumberDTO> list = new();

			var response = await _villaNumberService.GetAllAsync<Response>();
			if (response is not null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Data));
			}
			return View(list);
		}
	}
}