using AutoMapper;
using MagicVilla.API.Logging;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla.API.Controllers
{
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private ILogger<VillaAPIController> _logger;
		private readonly ILogging _customLogger;
		private readonly IVillaRepository _villaRepository;
		private readonly IMapper _mapper;
		private readonly Response _response;

		public VillaAPIController(ILogger<VillaAPIController> logger, ILogging customLogger, IVillaRepository villaRepository, IMapper mapper)
		{
			_logger = logger;
			_customLogger = customLogger;
			_villaRepository = villaRepository; 
			_mapper = mapper;
			_response = new();
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<Response>> GetVillas()
		{
			try
			{
				_logger.LogInformation("User fetched the list of villas.");
				_customLogger.Log("User fetched the list of villas.", "information");
				IEnumerable<Villa> villas = await _villaRepository.GetAllAsync();

				var data = _mapper.Map<IEnumerable<Villa>, IEnumerable<VillaDTO>>(villas);
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
				_response.Data = data;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>(){ Convert.ToString(ex.Message) };
			}
			return _response;
		}

		[HttpGet("{id}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Response>> GetVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_logger.LogError($"User made the bad request with Id: {id}.");
					_customLogger.Log($"User made the bad request with Id: {id}.", "error");
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return _response;
				}

				var villa = await _villaRepository.GetAsync(v => v.Id == id);
				if (villa is null)
				{
					_logger.LogInformation($"Villa not found of Id: {id}.");
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					_response.Data = villa;
					return _response;
				}

				_logger.LogInformation($"Villa fetched by user with Id: {id}.");
				var data = _mapper.Map<VillaDTO>(villa);
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
		public async Task<ActionResult<Response>> CreateVilla([FromBody] VillaCreateDTO villa)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					_logger.LogError($"User made the bad request as model state is not valid.");
					return BadRequest(ModelState);
				}

				if (await _villaRepository.GetAsync(v => v.Name == villa.Name) is not null)
				{
					_logger.LogError($"User made the bad request with Name: {villa.Name} because Villa already exists.");
					ModelState.AddModelError("ErrorMessages", "Villa already exists!");
					return BadRequest(ModelState);
				}

				if (villa is null)
				{
					_logger.LogError($"User made the bad request with null villa object.");
					return BadRequest(villa);
				}

				//var newVilla = new Villa()
				//{
				//	Name = villa.Name,
				//	Details = villa.Details,
				//	Rate = villa.Rate,
				//	Amenity = villa.Amenity,
				//	ImageURL = villa.ImageURL,
				//	Occupancy = villa.Occupancy,
				//	SqFt = villa.SqFt,
				//	CreatedDate = DateTime.Now,
				//};

				var newVilla = _mapper.Map<Villa>(villa);
				newVilla.CreatedDate = DateTime.Now;
				await _villaRepository.CreateAsync(newVilla);
				_logger.LogInformation($"Villa created by user of Id: {newVilla.Id}.");

				_response.StatusCode = HttpStatusCode.Created;
				_response.IsSuccess = true;
				_response.Data = newVilla;
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.InternalServerError;
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { Convert.ToString(ex.Message) };
			}
			return _response;
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Response>> DeleteVilla(int id)
		{
			try
			{
				if (id <= 0)
				{
					_logger.LogError($"User made the bad request by providing this Id: {id}");
					return BadRequest();
				}

				var villa = await _villaRepository.GetAsync(v => v.Id == id);
				if (villa is null)
				{
					_logger.LogError($"User made the bad request as villa not found with the provided Id: {id}");
					return NotFound();
				}

				await _villaRepository.RemoveAsync(villa);
				_logger.LogInformation($"Villa removed by user of Id: {id}.");

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

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<ActionResult<Response>> UpdateVilla(int id, [FromBody] VillaUpdateDTO villa)
		{
			try
			{
				if (villa is null || id != villa.Id)
				{
					_logger.LogError($"User made the bad request by providing the invalid villa: {villa} and id: {id}.");
					return BadRequest();
				}

				if (await _villaRepository.GetAsync(v => v.Id == id, tracked: false) is null)
				{
					_logger.LogError($"Villa not found with Id: {id}.");
					return NotFound();
				}

				//oldVilla.Id = villa.Id;
				//oldVilla.Name = villa.Name;
				//oldVilla.Details = villa.Details;
				//oldVilla.Rate = villa.Rate;
				//oldVilla.Amenity = villa.Amenity;
				//oldVilla.ImageURL = villa.ImageURL;
				//oldVilla.Occupancy = villa.Occupancy;
				//oldVilla.SqFt = villa.SqFt;
				//oldVilla.UpdatedDate = DateTime.Now;

				var newVilla = _mapper.Map<Villa>(villa);
				newVilla.UpdatedDate = DateTime.Now;

				var result = await _villaRepository.UpdateAsync(newVilla);
				_logger.LogInformation($"Villa updated by user of Id: {id}.");

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

		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Response>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> document)
		{
			try
			{
				if (id == 0 || document is null)
				{
					return BadRequest();
				}

				var villa = await _villaRepository.GetAsync(v => v.Id == id, tracked: false);
				if (villa is null)
				{
					return NotFound();
				}

				var oldVillaDTO = _mapper.Map<VillaUpdateDTO>(villa);
				//var oldVillaDTO = new VillaUpdateDTO()
				//{
				//	Id = villa.Id,
				//	Name = villa.Name,
				//	Details = villa.Details,
				//	Rate = villa.Rate,
				//	Amenity = villa.Amenity,
				//	ImageURL = villa.ImageURL,
				//	Occupancy = villa.Occupancy,
				//	SqFt = villa.SqFt,
				//};

				document.ApplyTo(oldVillaDTO, ModelState);

				if (ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				//var newVilla = new Villa()
				//{
				//	Id = oldVillaDTO.Id,
				//	Name = oldVillaDTO.Name,
				//	Details = oldVillaDTO.Details,
				//	Rate = oldVillaDTO.Rate,
				//	Amenity = oldVillaDTO.Amenity,
				//	ImageURL = oldVillaDTO.ImageURL,
				//	Occupancy = oldVillaDTO.Occupancy,
				//	SqFt = oldVillaDTO.SqFt,
				//	UpdatedDate = DateTime.Now
				//};

				var newVilla = _mapper.Map<Villa>(oldVillaDTO);
				newVilla.UpdatedDate = DateTime.Now;

				var result = await _villaRepository.UpdateAsync(newVilla);

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