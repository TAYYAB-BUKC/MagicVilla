using AutoMapper;
using MagicVilla.API.Controllers.v1;
using MagicVilla.API.Logging;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static MagicVilla.Utility.Configuration;

namespace MagicVilla.API.Controllers.v2
{
	[Route("api/v{version:apiVersion}/VillaAPI")]
	[ApiController]
	[ApiVersion("2.0")]
	public class VillaAPIv2Controller : ControllerBase
	{
		private readonly IVillaRepository _villaRepository;
		private readonly IMapper _mapper;
		private readonly Response _response;

		public VillaAPIv2Controller(IVillaRepository villaRepository, IMapper mapper)
		{
			_villaRepository = villaRepository;
			_mapper = mapper;
			_response = new();
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ResponseCache(CacheProfileName = CacheProfileName)]
		public async Task<ActionResult<Response>> GetVillas([FromQuery(Name = "FilterVillasByOccupany")] int? occupancy, [FromQuery(Name = "SearchVillaByName")] string? search, int pageSize = 3, int pageNumber = 1)
		{
			try
			{
				IEnumerable<Villa> villas;

				if (occupancy > 0)
				{
					villas = await _villaRepository.GetAllAsync(v => v.Occupancy == occupancy);
				}
				else
				{
					villas = await _villaRepository.GetAllAsync();
				}

				if (!string.IsNullOrWhiteSpace(search))
				{
					villas = villas.Where(v => v.Name.ToLower().Contains(search.ToLower())).ToList();
				}

				villas = villas.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

				Response.Headers.Add("X-Pagination", $"{{\"PageSize\":{pageSize}, \"PageNumber\":{pageNumber}}}");

				var data = _mapper.Map<IEnumerable<Villa>, IEnumerable<VillaDTO>>(villas);
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

		[HttpGet("GetString")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public IEnumerable<string> GetVillasVersion2()
		{
			return new List<string>()
			{
				"MagicString1",
				"MagicString2"
			};
		}

		[HttpGet("{id}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[Authorize(Roles = "admin")]
		[ResponseCache(CacheProfileName = CacheProfileName)]
		public async Task<ActionResult<Response>> GetVilla(int id)
		{
			try
			{
				if (id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return _response;
				}

				var villa = await _villaRepository.GetAsync(v => v.Id == id);
				if (villa is null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					_response.Data = villa;
					return _response;
				}

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
		public async Task<ActionResult<Response>> CreateVilla([FromForm] VillaCreateDTO villa)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				if (await _villaRepository.GetAsync(v => v.Name == villa.Name) is not null)
				{
					ModelState.AddModelError("ErrorMessages", "Villa already exists!");
					return BadRequest(ModelState);
				}

				if (villa is null)
				{
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

				if(villa.Image is not null)
				{
					string filename = $"{Convert.ToString(newVilla.Id)}{Path.GetExtension(villa.Image.FileName)}";
					string directoryPath = $"wwwroot/client/villaimages/";
					string filePath = $"{directoryPath}{filename}";

					var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
					if (!Directory.Exists(directoryLocation))
					{
						Directory.CreateDirectory(directoryLocation);
					}

					var fileInfo= new FileInfo(filePath);

					if (fileInfo.Exists)
					{
						fileInfo.Delete();
					}

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await villa.Image.CopyToAsync(fileStream);
					}

					var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

					newVilla.ImageURL = $"{baseURL}/client/villaimages/{filename}";
					newVilla.ImageLocalPath = filePath;
				}
				else
				{
					villa.ImageURL = $"https://placehold.co/600x40{newVilla.Id}";
				}

				await _villaRepository.UpdateAsync(newVilla);
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
		[Authorize(Roles = "admin")]
		public async Task<ActionResult<Response>> DeleteVilla(int id)
		{
			try
			{
				if (id <= 0)
				{
					return BadRequest();
				}

				var villa = await _villaRepository.GetAsync(v => v.Id == id);
				if (villa is null)
				{
					return NotFound();
				}

				await _villaRepository.RemoveAsync(villa);

				if (!String.IsNullOrEmpty(villa.ImageLocalPath))
				{
					var oldDirectoryLocation = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPath);

					var oldFileInfo = new FileInfo(oldDirectoryLocation);

					if (oldFileInfo.Exists)
					{
						oldFileInfo.Delete();
					}
				}

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
		public async Task<ActionResult<Response>> UpdateVilla(int id, [FromForm] VillaUpdateDTO villa)
		{
			try
			{
				if (villa is null || id != villa.Id)
				{
					return BadRequest();
				}

				if (await _villaRepository.GetAsync(v => v.Id == id, tracked: false) is null)
				{
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

				if (villa.Image is not null)
				{
					if (!String.IsNullOrEmpty(villa.ImageLocalPath))
					{
						var oldDirectoryLocation = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPath);
						
						var oldFileInfo = new FileInfo(oldDirectoryLocation);

						if (oldFileInfo.Exists)
						{
							oldFileInfo.Delete();
						}
					}

					string filename = $"{Convert.ToString(newVilla.Id)}{Path.GetExtension(villa.Image.FileName)}";
					string directoryPath = "wwwroot/client/villaimages/";
					string filePath = $"{directoryPath}{filename}";

					var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), directoryPath);
					if (!Directory.Exists(directoryLocation))
					{
						Directory.CreateDirectory(directoryLocation);
					}

					var fileInfo = new FileInfo(filePath);

					if (fileInfo.Exists)
					{
						fileInfo.Delete();
					}

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await villa.Image.CopyToAsync(fileStream);
					}

					var baseURL = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

					newVilla.ImageURL = $"{baseURL}/client/villaimages/{filename}";
					newVilla.ImageLocalPath = filePath;
				}
				else
				{
					villa.ImageURL = $"https://placehold.co/600x40{newVilla.Id}";
				}

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