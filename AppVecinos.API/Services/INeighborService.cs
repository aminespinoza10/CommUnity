using AppVecinos.API.Models;

namespace AppVecinos.API.Services
{
    public interface INeighborService
    {
        Task<IEnumerable<Neighbor>> GetNeighborsAsync();
        Task<Neighbor> GetNeighborByIdAsync(int id);
        Task<Neighbor> GetNeighborByCredentialsAsync(string username, string password);
        Task<Neighbor> CreateNeighborAsync(Neighbor neighbor);
        Task<Neighbor> UpdateNeighborAsync(Neighbor neighbor);
        Task DeleteNeighborAsync(int id);
    }
}