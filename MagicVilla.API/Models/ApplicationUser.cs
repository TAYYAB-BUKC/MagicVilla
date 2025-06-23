using Microsoft.AspNetCore.Identity;

namespace MagicVilla.API.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }
	}
}