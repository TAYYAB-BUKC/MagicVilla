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

		public ActionResult CreateVilla()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreateVilla(VillaCreateDTO villa)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaService.CreateAsync<Response>(villa);
				if (response is not null && response.IsSuccess)
				{
					TempData["Success"] = "Villa created successfully";
					return RedirectToAction(nameof(IndexVilla));
				}
			}

			TempData["Error"] = "Something went wrong while creating Villa.";
			return View(villa);
		}

		public async Task<ActionResult> UpdateVilla(int villaId)
		{
			var response = await _villaService.GetAsync<Response>(villaId);
			if (response is not null && response.IsSuccess)
			{
				var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Data));
				return View(_mapper.Map<VillaUpdateDTO>(model));
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateVilla(VillaUpdateDTO villa)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaService.UpdateAsync<Response>(villa);
				if (response is not null && response.IsSuccess)
				{
					TempData["Success"] = "Villa updated successfully";
					return RedirectToAction(nameof(IndexVilla));
				}
			}

			TempData["Error"] = "Something went wrong while updating Villa.";
			return View(villa);
		}

		public async Task<ActionResult> DeleteVilla(int villaId)
		{
			var response = await _villaService.GetAsync<Response>(villaId);
			if (response is not null && response.IsSuccess)
			{
				var model = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Data));
				return View(model);
			}
			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteVilla(VillaDTO villa)
		{
			var response = await _villaService.DeleteAsync<Response>(villa.Id);
			if (response is not null && response.IsSuccess)
			{
				TempData["Success"] = "Villa deleted successfully.";
				return RedirectToAction(nameof(IndexVilla));
			}

			TempData["Error"] = "Something went wrong while deleting Villa.";
			return View(villa);
		}
	}
}