using MagicVilla.API.Data;
using MagicVilla.API.Models;
using MagicVilla.API.Repository.Interfaces;

namespace MagicVilla.API.Repository
{
	public class VillaRepository : GenericRepository<Villa>, IVillaRepository
	{
		private readonly ApplicationDbContext _dbContext;
		
		public VillaRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<Villa> UpdateAsync(Villa entity)
		{
			_dbContext.Villas.Update(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}
	}
}