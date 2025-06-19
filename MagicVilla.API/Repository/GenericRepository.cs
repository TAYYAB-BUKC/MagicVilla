using MagicVilla.API.Data;
using MagicVilla.API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla.API.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly ApplicationDbContext _dbContext;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
			_dbSet = dbContext.Set<T>();
		}

		public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filters = null, string? includedProperties = null)
		{
			IQueryable<T> query = _dbSet;

			if (filters != null)
			{
				query = query.Where(filters);
			}

			if(includedProperties != null)
			{
				foreach(var property in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}

			return await query.ToListAsync();
		}

		public async Task<T> GetAsync(Expression<Func<T, bool>> filters = null, bool tracked = true, string? includedProperties = null)
		{
			IQueryable<T> query = _dbSet;

			if (!tracked)
			{
				query = query.AsNoTracking();
			}

			if (filters != null)
			{
				query = query.Where(filters);
			}

			if (includedProperties != null)
			{
				foreach (var property in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}

			return await query.FirstOrDefaultAsync();
		}

		public async Task CreateAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			await SaveAsync();
		}

		public async Task RemoveAsync(T entity)
		{
			_dbSet.Remove(entity);
			await SaveAsync();
		}

		public async Task SaveAsync()
		{
			await _dbContext.SaveChangesAsync();
		}
	}
}
