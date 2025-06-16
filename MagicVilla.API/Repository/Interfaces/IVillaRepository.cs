using MagicVilla.API.Models;
using System.Linq.Expressions;

namespace MagicVilla.API.Repository.Interfaces
{
	public interface IVillaRepository
	{
		Task<List<Villa>> GetAll(Expression<Func<Villa, bool>> filters = null!);
		Task<Villa> GetById(Expression<Func<Villa, bool>> filters = null!, bool tracked = true);
		Task Create(Villa entity);
		Task Remove(Villa entity);
		Task Save();
	}
}