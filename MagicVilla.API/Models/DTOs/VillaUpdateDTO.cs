using System.ComponentModel.DataAnnotations;

namespace MagicVilla.API.Models.DTOs
{
	public class VillaUpdateDTO
	{
		[Required]
		public int Id { get; set; }
		[Required]
		[MaxLength(50)]
		public string? Name { get; set; }
		[Required]
		public int SqFt { get; set; }
		[Required]
		public int Occupancy { get; set; }
		public string? Details { get; set; }
		[Required]
		public double Rate { get; set; }
		public string? ImageURL { get; set; }
		public string? Amenity { get; set; }
	}
}