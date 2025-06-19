using AutoMapper;
using MagicVilla.Web.Models;
using MagicVilla.Web.Models.DTOs;
using MagicVilla.Web.Models.ViewModels;
using MagicVilla.Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Reflection;

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
		public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM villa)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.CreateAsync<Response>(villa.VillaNumber);
				if (response is not null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					if (response.ErrorMessages.Any())
					{
						foreach (var errorMessage in response.ErrorMessages)
						{
							ModelState.AddModelError("ErrorMessages", errorMessage);
						}
					}
				}
			}

			var villaResponse = await _villaService.GetAllAsync<Response>();
			if (villaResponse is not null && villaResponse.IsSuccess)
			{
				villa.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
								  (Convert.ToString(villaResponse.Data)).Select(v => new SelectListItem
								  {
									  Text = v.Name,
									  Value = Convert.ToString(v.Id)
								  });
			}


			return View(villa);
		}

		public async Task<ActionResult> UpdateVillaNumber(int villaNo)
		{
			VillaNumberUpdateVM viewModel = new();
			var response = await _villaNumberService.GetAsync<Response>(villaNo);
			if (response is not null && response.IsSuccess)
			{
				var model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Data));
				viewModel.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
			}

			var villaResponse = await _villaService.GetAllAsync<Response>();
			if (villaResponse is not null && villaResponse.IsSuccess)
			{
				viewModel.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
								  (Convert.ToString(villaResponse.Data)).Select(v => new SelectListItem
								  {
									  Text = v.Name,
									  Value = Convert.ToString(v.Id)
								  });
				return View(viewModel);
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM villa)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.UpdateAsync<Response>(villa.VillaNumber);
				if (response is not null && response.IsSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					if (response.ErrorMessages.Any())
					{
						foreach (var errorMessage in response.ErrorMessages)
						{
							ModelState.AddModelError("ErrorMessages", errorMessage);
						}
					}
				}
			}

			var villaResponse = await _villaService.GetAllAsync<Response>();
			if (villaResponse is not null && villaResponse.IsSuccess)
			{
				villa.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
								  (Convert.ToString(villaResponse.Data)).Select(v => new SelectListItem
								  {
									  Text = v.Name,
									  Value = Convert.ToString(v.Id)
								  });
			}


			return View(villa);
		}

		public async Task<ActionResult> DeleteVillaNumber(int villaId)
		{
			VillaNumberDeleteVM viewModel = new();
			var response = await _villaService.GetAsync<Response>(villaId);
			if (response is not null && response.IsSuccess)
			{
				viewModel.VillaNumber = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Data));
				var villaResponse = await _villaService.GetAllAsync<Response>();
				if (villaResponse is not null && villaResponse.IsSuccess)
				{
					viewModel.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
									  (Convert.ToString(villaResponse.Data)).Select(v => new SelectListItem
									  {
										  Text = v.Name,
										  Value = Convert.ToString(v.Id)
									  });
					return View(viewModel);
				}
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM villa)
		{
			var response = await _villaService.DeleteAsync<Response>(villa.VillaNumber.VillaNo);
			if (response is not null && response.IsSuccess)
			{
				return RedirectToAction(nameof(IndexVillaNumber));
			}
			else
			{
				if (response.ErrorMessages.Any())
				{
					foreach (var errorMessage in response.ErrorMessages)
					{
						ModelState.AddModelError("ErrorMessages", errorMessage);
					}
				}
			}
			var villaResponse = await _villaService.GetAllAsync<Response>();
			if (villaResponse is not null && villaResponse.IsSuccess)
			{
				villa.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
								  (Convert.ToString(villaResponse.Data)).Select(v => new SelectListItem
								  {
									  Text = v.Name,
									  Value = Convert.ToString(v.Id)
								  });
			}

			return View(villa);
		}
	}
}