using System.Linq.Expressions;

namespace MagicVilla.API.Repository.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filters = null!, string? includedProperties = null);
		Task<T> GetAsync(Expression<Func<T, bool>> filters = null!, bool tracked = true, string? includedProperties = null);
		Task CreateAsync(T entity);
		Task RemoveAsync(T entity);
		Task SaveAsync();
	}
}