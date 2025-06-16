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

		public async Task<List<Villa>> GetAll(Expression<Func<Villa, bool>> filters = null)
		{
			IQueryable<Villa> query = _dbContext.Villas;

			if (filters != null)
			{
				query = query.Where(filters);
			}

			return await query.ToListAsync();
		}

		public async Task<Villa> GetById(Expression<Func<Villa, bool>> filters = null, bool tracked = true)
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

		public async Task Create(Villa entity)
		{
			await _dbContext.Villas.AddAsync(entity);
			await Save();
		}

		public async Task Remove(Villa entity)
		{
			_dbContext.Villas.Remove(entity);
			await Save();
		}

		public async Task Save()
		{
			await _dbContext.SaveChangesAsync();
		}
	}
}