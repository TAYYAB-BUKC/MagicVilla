using MagicVilla.API.Data;
using MagicVilla.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla.API.Controllers
{
	//[Route("api/[controller]")]
	[Route("api/VillaAPI")]
	[ApiController]
	public class VillaAPIController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<Villa> GetVillas()
		{
			return VillaStore.VillaList;
		}
	}
}