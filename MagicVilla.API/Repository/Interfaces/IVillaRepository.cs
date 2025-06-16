using MagicVilla.API.Models;
using System.Linq.Expressions;

namespace MagicVilla.API.Repository.Interfaces
{
	public interface IVillaRepository
	{
		Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filters = null!);
		Task<Villa> GetAsync(Expression<Func<Villa, bool>> filters = null!, bool tracked = true);
		Task CreateAsync(Villa entity);
		Task UpdateAsync(Villa entity);
		Task RemoveAsync(Villa entity);
		Task SaveAsync();
	}
}