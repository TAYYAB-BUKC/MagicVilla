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
			return new List<Villa>()
			{
				new Villa { Id = 1, Name = "Pool Villa" },
				new Villa { Id = 2, Name = "Pool Villa" }
			};
		}
	}
}