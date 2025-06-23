using AutoMapper;
using MagicVilla.API.Controllers.v1;
using MagicVilla.API.Logging;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
	}
}