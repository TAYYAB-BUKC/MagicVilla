using MagicVilla.API.Data;
using MagicVilla.API.Models;
using MagicVilla.API.Repository.Interfaces;

namespace MagicVilla.API.Repository
{
	public class VillaNumberRepository : GenericRepository<VillaNumber>, IVillaNumberRepository
	{
		public readonly ApplicationDbContext _dbContext;
		public VillaNumberRepository(ApplicationDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<VillaNumber> UpdateAsync(VillaNumber entity)
		{
			_dbContext.VillaNumbers.Update(entity);
			await _dbContext.SaveChangesAsync();
			return entity;
		}
	}
}