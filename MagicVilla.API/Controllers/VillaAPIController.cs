using MagicVilla.API.Data;
using MagicVilla.API.Logging;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.API.Controllers
{
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private ILogger<VillaAPIController> _logger;
		private readonly ILogging _customLogger;
		private readonly ApplicationDbContext _dbContext;

		public VillaAPIController(ILogger<VillaAPIController> logger, ILogging customLogger, ApplicationDbContext dbContext)
		{
			_logger = logger;
			_customLogger = customLogger;
			_dbContext = dbContext;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			_logger.LogInformation("User fetched the list of villas.");
			_customLogger.Log("User fetched the list of villas.", "information");
			return Ok(_dbContext.Villas.ToList());
		}

		[HttpGet("{id}", Name = "GetVilla")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetVilla(int id)
		{
			if (id == 0)
			{
				_logger.LogError($"User made the bad request with Id: {id}.");
				_customLogger.Log($"User made the bad request with Id: {id}.", "error");
				return BadRequest();
			}

			var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
			if (villa is null)
			{
				_logger.LogInformation($"Villa not found of Id: {id}.");
				return NotFound();
			}

			_logger.LogInformation($"Villa fetched by user with Id: {id}.");
			return Ok(villa);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult CreateVilla([FromBody] VillaCreateDTO villa)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogError($"User made the bad request as model state is not valid.");
				return BadRequest(ModelState);
			}

			if (_dbContext.Villas.FirstOrDefault(v => v.Name == villa.Name) is not null)
			{ 
				_logger.LogError($"User made the bad request with Name: {villa.Name} because Villa already exists.");
				ModelState.AddModelError("CustomError", "Villa already exists!");
				return BadRequest(ModelState);
			}

			if (villa is null)
			{
				_logger.LogError($"User made the bad request with null villa object.");
				return BadRequest(villa);
			}

			var newVilla = new Villa()
			{
				Name = villa.Name,
				Details = villa.Details,
				Rate = villa.Rate,
				Amenity = villa.Amenity,
				ImageURL = villa.ImageURL,
				Occupancy = villa.Occupancy,
				SqFt = villa.SqFt,
				CreatedDate = DateTime.Now,
			};
			
			_dbContext.Villas.Add(newVilla);
			_dbContext.SaveChangesAsync();
			_logger.LogInformation($"Villa created by user of Id: {newVilla.Id}.");
			return CreatedAtRoute("GetVilla", new { id = newVilla.Id }, newVilla);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public IActionResult DeleteVilla(int id)
		{
			if (id <= 0)
			{
				_logger.LogError($"User made the bad request by providing this Id: {id}");
				return BadRequest();
			}

			var villa = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
			if (villa is null)
			{
				_logger.LogError($"User made the bad request as villa not found with the provided Id: {id}");
				return NotFound();
			}

			_dbContext.Villas.Remove(villa);
			_dbContext.SaveChangesAsync();
			_logger.LogInformation($"Villa removed by user of Id: {id}.");
			return NoContent();
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult UpdateVilla(int id, [FromBody] VillaUpdateDTO villa)
		{
			if (villa is null || id != villa.Id)
			{
				_logger.LogError($"User made the bad request by providing the invalid villa: {villa} and id: {id}.");
				return BadRequest();
			}

			var oldVilla = _dbContext.Villas.FirstOrDefault(v => v.Id == id);
			if (oldVilla is null)
			{
				_logger.LogError($"Villa not found with Id: {id}.");
				return NotFound();
			}

			oldVilla.Id = villa.Id;
			oldVilla.Name = villa.Name;
			oldVilla.Details = villa.Details;
			oldVilla.Rate = villa.Rate;
			oldVilla.Amenity = villa.Amenity;
			oldVilla.ImageURL = villa.ImageURL;
			oldVilla.Occupancy = villa.Occupancy;
			oldVilla.SqFt = villa.SqFt;
			oldVilla.UpdatedDate = DateTime.Now;
			
			_dbContext.Villas.Update(oldVilla);
			_dbContext.SaveChangesAsync();
			_logger.LogInformation($"Villa updated by user of Id: {id}.");
			return NoContent();
		}

		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> document)
		{
			if (id == 0 || document is null)
			{
				return BadRequest();
			}

			var villa = _dbContext.Villas.AsNoTracking().FirstOrDefault(v => v.Id == id);
			if (villa is null)
			{
				return NotFound();
			}

			var oldVillaDTO = new VillaUpdateDTO()
			{
				Id = villa.Id,
				Name = villa.Name,
				Details = villa.Details,
				Rate = villa.Rate,
				Amenity = villa.Amenity,
				ImageURL = villa.ImageURL,
				Occupancy = villa.Occupancy,
				SqFt = villa.SqFt,
			};

			document.ApplyTo(oldVillaDTO, ModelState);

			if (ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var newVilla = new Villa()
			{
				Id = oldVillaDTO.Id,
				Name = oldVillaDTO.Name,
				Details = oldVillaDTO.Details,
				Rate = oldVillaDTO.Rate,
				Amenity = oldVillaDTO.Amenity,
				ImageURL = oldVillaDTO.ImageURL,
				Occupancy = oldVillaDTO.Occupancy,
				SqFt = oldVillaDTO.SqFt,
				UpdatedDate = DateTime.Now
			};
			
			_dbContext.Villas.Update(newVilla);
			_dbContext.SaveChangesAsync();
			
			return NoContent();
		}
	}
}