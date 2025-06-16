using System.Linq.Expressions;

namespace MagicVilla.API.Repository.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filters = null!);
		Task<T> GetAsync(Expression<Func<T, bool>> filters = null!, bool tracked = true);
		Task CreateAsync(T entity);
		Task RemoveAsync(T entity);
		Task SaveAsync();
	}
}