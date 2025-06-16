using AutoMapper;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla.API.Controllers
{
	[Route("api/VillaNumberAPI")]
	[ApiController]
	public class VillaNumberAPIController : ControllerBase
	{
		private readonly IVillaNumberRepository _villaNumberRepository;
		private readonly IVillaRepository _villaRepository;
		private readonly IMapper _mapper;
		private readonly Response _response;

		public VillaNumberAPIController(IVillaNumberRepository villaNumberRepository, IMapper mapper, IVillaRepository villaRepository)
		{
			_villaNumberRepository = villaNumberRepository;
			_mapper = mapper;
			_response = new();
			_villaRepository = villaRepository;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<Response>> GetVillaNumbers()
		{
			try
			{
				IEnumerable<VillaNumber> villaNumbers = await _villaNumberRepository.GetAllAsync();

				var data = _mapper.Map<IEnumerable<VillaNumberDTO>>(villaNumbers);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				_response.Data = data;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
			}
			return _response;
		}

		[HttpGet("{villaNo}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Response>> GetVillaNumber(int villaNo)
		{
			try
			{
				if (villaNo == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return _response;
				}

				var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo);
				if (villaNumber is null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					_response.Data = villaNumber;
					return _response;
				}

				var data = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				_response.Data = data;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
			}
			return _response;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Response>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaNumber)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				if (villaNumber is null)
				{
					return BadRequest(villaNumber);
				}

				if (await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNumber.VillaNo) is not null)
				{
					ModelState.AddModelError("CustomError", "VillaNumber already exists!");
					return BadRequest(ModelState);
				}

				if (await _villaRepository.GetAsync(v => v.Id == villaNumber.VillaID) is null)
				{
					ModelState.AddModelError("CustomError", "Villa does not exists!");
					return BadRequest(ModelState);
				}

				var newVillaNumber = _mapper.Map<VillaNumber>(villaNumber);
				newVillaNumber.CreatedDate = DateTime.Now;
				await _villaNumberRepository.CreateAsync(newVillaNumber);

				_response.StatusCode = HttpStatusCode.Created;
				_response.IsSuccess = true;
				_response.Data = newVillaNumber;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
			}
			return _response;
		}

		[HttpDelete("{villaNo}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Response>> DeleteVillaNumber(int villaNo)
		{
			try
			{
				if (villaNo <= 0)
				{
					return BadRequest();
				}

				var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo);
				if (villaNumber is null)
				{
					return NotFound();
				}

				await _villaNumberRepository.RemoveAsync(villaNumber);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
			}
			return _response;
		}

		[HttpPut("{villaNo}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Response>> UpdateVillaNumber(int villaNo, [FromBody] VillaNumberUpdateDTO villaNumber)
		{
			try
			{
				if (villaNumber is null || villaNo != villaNumber.VillaNo)
				{
					return BadRequest();
				}

				if (await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo) is null)
				{
					return NotFound();
				}

				if (await _villaRepository.GetAsync(v => v.Id == villaNumber.VillaID) is null)
				{
					ModelState.AddModelError("CustomError", "Villa does not exists!");
					return BadRequest(ModelState);
				}

				var newVillaNumber = _mapper.Map<VillaNumber>(villaNumber);
				newVillaNumber.UpdatedDate = DateTime.Now;

				var result = await _villaNumberRepository.UpdateAsync(newVillaNumber);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				_response.Data = result;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
			}
			return _response;
		}

		[HttpPatch("{villaNo:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Response>> UpdatePartialVilla(int villaNo, JsonPatchDocument<VillaNumberUpdateDTO> document)
		{
			try
			{
				if (villaNo == 0 || document is null)
				{
					return BadRequest();
				}

				var villaNumber = await _villaNumberRepository.GetAsync(v => v.VillaNo == villaNo, tracked: false);
				if (villaNumber is null)
				{
					return NotFound();
				}

				var oldVillaNumberDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);
				document.ApplyTo(oldVillaNumberDTO, ModelState);

				if (ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				if (await _villaRepository.GetAsync(v => v.Id == villaNumber.VillaID) is null)
				{
					ModelState.AddModelError("CustomError", "Villa does not exists!");
					return BadRequest(ModelState);
				}

				var newVillaNumber = _mapper.Map<VillaNumber>(oldVillaNumberDTO);
				newVillaNumber.UpdatedDate = DateTime.Now;

				var result = await _villaNumberRepository.UpdateAsync(newVillaNumber);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				_response.Data = result;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
			}

			return _response;
		}
	}
}