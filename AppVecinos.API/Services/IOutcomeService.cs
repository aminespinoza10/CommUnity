using AppVecinos.API.Models;

namespace AppVecinos.API.Services
{
    public interface IOutcomeService
    {
        Task<IEnumerable<Outcome>> GetOutcomesAsync();
        Task<Outcome> CreateOutcomeAsync(Outcome outcome);
        Task<IEnumerable<Outcome>> GetOutcomesByYearAsync(string year);
        Task<IEnumerable<Outcome>> GetOutcomesByMonthAsync(string month);   
    }
}