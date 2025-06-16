using MagicVilla.API.Models;

namespace MagicVilla.API.Repository.Interfaces
{
	public interface IVillaNumberRepository : IGenericRepository<VillaNumber>
	{
		Task<VillaNumber> UpdateAsync(VillaNumber entity);
	}
}