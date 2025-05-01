using MagicVilla.API.Data;
using MagicVilla.API.Models;
using MagicVilla.API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.API.Controllers
{
	//[Route("api/[controller]")]
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public ActionResult<IEnumerable<VillaDTO>> GetVillas()
		{
			return VillaStore.VillaList;
		}

		[HttpGet("{id}")]
		//[ProducesResponseType(200)]
		//[ProducesResponseType(200, Type = typeof(VillaDTO))]
		//[ProducesResponseType(400)]
		//[ProducesResponseType(404)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public ActionResult GetVilla(int id)
		{
			if (id == 0)
			{
				return BadRequest();
			}

			var villa = VillaStore.VillaList.Find(v => v.Id == id);
			if(villa is null)
			{
				return NotFound();
			}

			return Ok(villa);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public ActionResult CreateVilla([FromBody] VillaDTO villa)
		{
			if (villa is null)
			{
				return BadRequest(villa);
			}

			if (villa.Id > 0)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			villa.Id = (VillaStore.VillaList.OrderByDescending(v => v.Id).FirstOrDefault()).Id + 1;	
			VillaStore.VillaList.Add(villa);

			return Ok(villa);
		}
	}
}