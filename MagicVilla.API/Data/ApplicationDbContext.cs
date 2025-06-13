using MagicVilla.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.API.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{
			
		}

		public DbSet<Villa> Villas { get; set; }
	}
}