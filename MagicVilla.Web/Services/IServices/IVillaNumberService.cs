using MagicVilla.Web.Models.DTOs;

namespace MagicVilla.Web.Services.IServices
{
	public interface IVillaNumberService
	{
		Task<T> GetAllAsync<T>();
		Task<T> GetAsync<T>(int id);
		Task<T> CreateAsync<T>(VillaNumberCreateDTO villa);
		Task<T> UpdateAsync<T>(VillaNumberUpdateDTO villa);
		Task<T> DeleteAsync<T>(int id);
	}
}