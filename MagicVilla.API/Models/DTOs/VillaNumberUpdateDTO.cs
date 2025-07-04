﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla.API.Models.DTOs
{
	public class VillaNumberUpdateDTO
	{
		[Required]
		public int VillaNo { get; set; }
		[Required]
		public int VillaID { get; set; }
		public string? SpecialDetails { get; set; }
	}
}
