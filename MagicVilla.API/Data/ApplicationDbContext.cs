﻿using MagicVilla.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.API.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Villa> Villas { get; set; }

		public DbSet<VillaNumber> VillaNumbers { get; set; }

		public DbSet<LocalUser> LocalUsers { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Villa>().HasData(
				new Villa
				{
					Id = 1,
					Name = "Royal Villa",
					Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
					ImageURL = "https://dotnetmastery.com/bluevillaimages/villa3.jpg",
					Occupancy = 4,
					Rate = 200,
					SqFt = 550,
					Amenity = "",
					CreatedDate = new DateTime(2025, 6, 13, 17, 9, 3, 684, DateTimeKind.Local).AddTicks(9451)
				},
			  new Villa
			  {
				  Id = 2,
				  Name = "Premium Pool Villa",
				  Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
				  ImageURL = "https://dotnetmastery.com/bluevillaimages/villa1.jpg",
				  Occupancy = 4,
				  Rate = 300,
				  SqFt = 550,
				  Amenity = "",
				  CreatedDate = new DateTime(2025, 6, 13, 17, 9, 3, 684, DateTimeKind.Local).AddTicks(9451)
			  },
			  new Villa
			  {
				  Id = 3,
				  Name = "Luxury Pool Villa",
				  Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
				  ImageURL = "https://dotnetmastery.com/bluevillaimages/villa4.jpg",
				  Occupancy = 4,
				  Rate = 400,
				  SqFt = 750,
				  Amenity = "",
				  CreatedDate = new DateTime(2025, 6, 13, 17, 9, 3, 684, DateTimeKind.Local).AddTicks(9451)
			  },
			  new Villa
			  {
				  Id = 4,
				  Name = "Diamond Villa",
				  Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
				  ImageURL = "https://dotnetmastery.com/bluevillaimages/villa5.jpg",
				  Occupancy = 4,
				  Rate = 550,
				  SqFt = 900,
				  Amenity = "",
				  CreatedDate = new DateTime(2025, 6, 13, 17, 9, 3, 684, DateTimeKind.Local).AddTicks(9451)
			  },
			  new Villa
			  {
				  Id = 5,
				  Name = "Diamond Pool Villa",
				  Details = "Fusce 11 tincidunt maximus leo, sed scelerisque massa auctor sit amet. Donec ex mauris, hendrerit quis nibh ac, efficitur fringilla enim.",
				  ImageURL = "https://dotnetmastery.com/bluevillaimages/villa2.jpg",
				  Occupancy = 4,
				  Rate = 600,
				  SqFt = 1100,
				  Amenity = "",
				  CreatedDate = new DateTime(2025, 6, 13, 17, 9, 3, 684, DateTimeKind.Local).AddTicks(9451)
			  });
		}
	}
}