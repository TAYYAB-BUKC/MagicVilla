﻿namespace MagicVilla.API.Models.DTOs
{
	public class LoginResponseDTO
	{
		public required string AccessToken { get; set; }
		public required string RefreshToken { get; set; }
	}
}