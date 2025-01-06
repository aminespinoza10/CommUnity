using AppVecinos.API.Models;

namespace AppVecinos.API.Services
{
    public interface IFeeService
    {
        Task<IEnumerable<Fee>> GetFeesAsync();
        Task<Fee> GetFeeByIdAsync(int id);
        Task<Fee> CreateFeeAsync(Fee fee);
        Task<Fee> UpdateFeeAsync(Fee fee);
        Task DeleteFeeAsync(int id);
    }
}