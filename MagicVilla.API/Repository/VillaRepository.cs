using MagicVilla.API.Data;
using MagicVilla.API.Models;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla.API.Repository
{
	public class VillaRepository : IVillaRepository
	{
		private readonly ApplicationDbContext _dbContext;
		
		public VillaRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filters = null)
		{
			IQueryable<Villa> query = _dbContext.Villas;

			if (filters != null)
			{
				query = query.Where(filters);
			}

			return await query.ToListAsync();
		}

		public async Task<Villa> GetAsync(Expression<Func<Villa, bool>> filters = null, bool tracked = true)
		{
			IQueryable<Villa> query = _dbContext.Villas;

			if (!tracked)
			{
				query = query.AsNoTracking();
			}

			if(filters != null)
			{
				query = query.Where(filters);
			}

			return await query.FirstOrDefaultAsync();
		}

		public async Task CreateAsync(Villa entity)
		{
			await _dbContext.Villas.AddAsync(entity);
			await SaveAsync();
		}

		public async Task UpdateAsync(Villa entity)
		{
			_dbContext.Villas.Update(entity);
			await SaveAsync();
		}

		public async Task RemoveAsync(Villa entity)
		{
			_dbContext.Villas.Remove(entity);
			await SaveAsync();
		}

		public async Task SaveAsync()
		{
			await _dbContext.SaveChangesAsync();
		}
	}
}