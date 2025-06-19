using MagicVilla.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla.Web.Models.ViewModels
{
	public class VillaNumberUpdateVM
	{
		public VillaNumberUpdateDTO VillaNumber { get; set; } = new VillaNumberUpdateDTO();
		[ValidateNever]
		public IEnumerable<SelectListItem> VillaList { get; set; }
	}
}