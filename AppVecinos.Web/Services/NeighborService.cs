using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppVecinos.Web.Models;
using Microsoft.Extensions.Logging;

namespace AppVecinos.Web.Services;
public class NeighborService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NeighborService> _logger;

    public NeighborService(HttpClient httpClient, 
    ILogger<NeighborService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Neighbor>> GetNeighborsAsync(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync(ApiRoutes.Neighbors.NeighborsEndpoint);
            _logger.LogInformation($"RESPUESTA: {await response.Content.ReadAsStringAsync()}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var neighbors = JsonSerializer.Deserialize<IEnumerable<Neighbor>>(responseBody);               

                return neighbors;
            }
            else
            {
                throw new Exception("Unable to fetch neighbors");
            }
        }

        return new List<Neighbor>();
    }

    public async Task<Neighbor> AddNeighborAsync(Neighbor neighbor, string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var neighborJson = JsonSerializer.Serialize(neighbor);
            var content = new StringContent(neighborJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(ApiRoutes.Neighbors.NeighborsEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var neighborResponse = JsonSerializer.Deserialize<Neighbor>(responseBody);
                return neighborResponse;
            }
            else
            {
                throw new Exception("No fue posible agregar el vecino", new Exception(await response.Content.ReadAsStringAsync()));
            }
        }

        return null;
    }

    public  async Task<Neighbor> EditNeighborAsync(Neighbor neighbor, string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var neighborJson = JsonSerializer.Serialize(neighbor);
            var content = new StringContent(neighborJson, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(ApiRoutes.Neighbors.NeighborsEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var neighborResponse = JsonSerializer.Deserialize<Neighbor>(responseBody);
                return neighborResponse;
            }
            else
            {
                throw new Exception("No se pudo editar el vecino debido a un error del servidor.", new Exception(await response.Content.ReadAsStringAsync()));
            }
        }

        return null;
    }

    public async Task<Neighbor> GetNeighborByIdAsync(int id, string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"{ApiRoutes.Neighbors.NeighborsEndpoint}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var neighbor = JsonSerializer.Deserialize<Neighbor>(responseBody);
                if (neighbor == null)
                {
                    throw new Exception("El vecino deserializado es nulo.");
                }
                return neighbor;
            }
            else
            {
                throw new Exception("No fue posible obtener el vecino", new Exception(await response.Content.ReadAsStringAsync()));
            }
        }
       throw new ArgumentException("El token no puede ser nulo o vacío", nameof(token));       
    }

    public async Task DeleteNeighborAsync(int id, string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"{ApiRoutes.Neighbors.NeighborsEndpoint}/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("No fue posible eliminar el vecino", new Exception(await response.Content.ReadAsStringAsync()));
            }
            return;
        }
    }
}