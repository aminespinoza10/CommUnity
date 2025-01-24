using System.Collections.Generic;
using System.Threading.Tasks;
using AppVecinos.Web.Models;

namespace AppVecinos.Web.Services
{
    public interface INeighborService
    {
        Task<IEnumerable<Neighbor>> GetNeighborsAsync(string token);

    }
}