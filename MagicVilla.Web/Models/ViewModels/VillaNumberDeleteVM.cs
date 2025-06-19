using MagicVilla.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla.Web.Models.ViewModels
{
	public class VillaNumberDeleteVM
	{
		public VillaNumberDTO VillaNumber { get; set; } = new VillaNumberDTO();
		[ValidateNever]
		public IEnumerable<SelectListItem> VillaList { get; set; }
	}
}