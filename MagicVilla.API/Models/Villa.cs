﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla.API.Models
{
	public class Villa
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		public required string Name { get; set; }
		public string? Details { get; set; }
		public double Rate { get; set; }
		public string? ImageURL { get; set; }
		public string? ImageLocalPath { get; set; }
		public string? Amenity { get; set; }
		public int SqFt { get; set; }
		public int Occupancy { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime UpdatedDate { get; set; }
	}
}