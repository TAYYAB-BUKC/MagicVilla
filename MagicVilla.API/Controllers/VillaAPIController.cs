using MagicVilla.API.Data;
using MagicVilla.API.Logging;
using MagicVilla.API.Models.DTOs;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.API.Controllers
{
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		private ILogger<VillaAPIController> _logger;
		private readonly ILogging _customLogger;

		public VillaAPIController(ILogger<VillaAPIController> logger, ILogging customLogger)
		{
			_logger = logger;
			_customLogger = customLogger;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			_logger.LogInformation("User fetched the list of villas.");
			_customLogger.Log("User fetched the list of villas.", "information");
			return VillaStore.VillaList;
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

			var villa = VillaStore.GetVilla(id);
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
		public ActionResult CreateVilla([FromBody] VillaDTO villa)
		{
			if (!ModelState.IsValid)
			{
				_logger.LogError($"User made the bad request as model state is not valid.");
				return BadRequest(ModelState);
			}
			
			if(VillaStore.CheckExistingVillaByName(villa.Name) is not null)
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

			if (villa.Id > 0)
			{
				_logger.LogError($"User made the bad request by providing the villa Id on Create endpoint and Id is {villa.Id}.");
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			VillaStore.AddVilla(villa);

			_logger.LogInformation($"Villa created by user of Id: {villa.Id}.");
			return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
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

			var villa = VillaStore.GetVilla(id);
			if (villa is null)
			{
				_logger.LogError($"User made the bad request as villa not found with the provided Id: {id}");
				return NotFound();
			}

			VillaStore.DeleteVilla(villa);
			_logger.LogInformation($"Villa removed by user of Id: {id}.");
			return NoContent();
		}

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult UpdateVilla(int id, [FromBody] VillaDTO villa)
		{
			if (villa is null || id != villa.Id)
			{
				_logger.LogError($"User made the bad request by providing the invalid villa: {villa} and id: {id}.");
				return BadRequest();
			}

			var oldVilla = VillaStore.GetVilla(id);
			if (oldVilla is null)
			{
				_logger.LogError($"Villa not found with Id: {id}.");
				return NotFound();
			}

			VillaStore.UpdateVilla(oldVilla, villa);
			_logger.LogInformation($"Villa updated by user of Id: {id}.");
			return NoContent();
		}

		[HttpPatch("{id:int}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> document)
		{
			if (id == 0 || document is null)
			{
				return BadRequest();
			}

			var oldVilla = VillaStore.GetVilla(id);
			if (oldVilla is null)
			{
				return NotFound();
			}

			document.ApplyTo(oldVilla, ModelState);
			if (ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return NoContent();
		}
	}
}