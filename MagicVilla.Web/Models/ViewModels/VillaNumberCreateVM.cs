using MagicVilla.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MagicVilla.Web.Models.ViewModels
{
	public class VillaNumberCreateVM
	{
		public VillaNumberCreateDTO VillaNumber { get; set; } = new VillaNumberCreateDTO();
		[ValidateNever]
		public IEnumerable<SelectListItem> VillaList { get; set; }
	}
}