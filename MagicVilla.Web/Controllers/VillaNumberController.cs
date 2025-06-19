using AutoMapper;
using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Models.ViewModels;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MagicVilla.Web.Controllers
{
	public class VillaNumberController : Controller
	{
		public IVillaNumberService _villaNumberService { get; set; }
		public IVillaService _villaService { get; set; }
		public IMapper _mapper { get; set; }

		public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
		{
			_villaNumberService = villaNumberService;
			_mapper = mapper;
			_villaService = villaService;
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

		public async Task<ActionResult> CreateVillaNumber()
		{
			VillaNumberCreateVM model = new VillaNumberCreateVM();

			var response = await _villaService.GetAllAsync<Response>();
			if (response is not null && response.IsSuccess)
			{
				model.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
								  (Convert.ToString(response.Data)).Select(v => new SelectListItem
								  {
									  Text = v.Name,
									  Value = Convert.ToString(v.Id)
								  });
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateDTO villa)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.CreateAsync<Response>(villa);
				if (response is not null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
			}

			return View(villa);
		}
	}
}