using AppVecinos.API.Data;
using AppVecinos.API.Models;

namespace AppVecinos.API.Services
{
    public class NeighborService : INeighborService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NeighborService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Neighbor>> GetNeighborsAsync()
        {
            return await _unitOfWork.NeighborRepository.GetAllAsync();
        }

        public async Task<Neighbor> GetNeighborByIdAsync(int id)
        {
            return await _unitOfWork.NeighborRepository.GetByIdAsync(id);
        }

        public async Task<Neighbor> CreateNeighborAsync(Neighbor neighbor)
        {
            await _unitOfWork.NeighborRepository.AddAsync(neighbor);
            await _unitOfWork.SaveAsync();
            return neighbor;
        }

        public async Task<Neighbor> UpdateNeighborAsync(Neighbor neighbor)
        {
            if (await _unitOfWork.NeighborRepository.GetByIdAsync(neighbor.Id) == null)
            {
                return null;
            }
            _unitOfWork.NeighborRepository.Update(neighbor);
            await _unitOfWork.SaveAsync();
            return neighbor;
        }

        public async Task DeleteNeighborAsync(int id)
        {
            await _unitOfWork.NeighborRepository.Remove(id);
            await _unitOfWork.SaveAsync();
        }
    }
}