﻿namespace MagicVilla.API.Models.DTOs
{
	public class LoginRequestDTO
	{
		public required string Username { get; set; }
		public required string Password { get; set; }
	}
}