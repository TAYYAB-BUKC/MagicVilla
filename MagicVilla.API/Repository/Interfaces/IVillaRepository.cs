using MagicVilla.API.Models;

namespace MagicVilla.API.Repository.Interfaces
{
	public interface IVillaRepository : IGenericRepository<Villa>
	{
		Task<Villa> UpdateAsync(Villa entity);
	}
}