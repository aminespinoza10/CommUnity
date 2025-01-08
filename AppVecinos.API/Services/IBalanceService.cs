using AppVecinos.API.Models;

namespace AppVecinos.API.Services
{
    public interface IBalanceService
    {
        Task<IEnumerable<Balance>> GetBalancesAsync();
        Task<Balance> CreateBalanceAsync(Balance balance);
        Task<IEnumerable<Balance>> GetBalancesByYearAsync(string year);
        Task<IEnumerable<Balance>> GetBalancesByPeriodAsync(string period);   
    }
}