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
	}
}