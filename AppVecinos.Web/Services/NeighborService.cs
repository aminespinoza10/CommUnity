using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AppVecinos.Web.Models;
using Microsoft.Extensions.Logging;

namespace AppVecinos.Web.Services;
public class NeighborService: INeighborService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor  _httpContextAccessor;
    private readonly ILogger<NeighborService> _logger;

    public NeighborService(HttpClient httpClient, 
    IHttpContextAccessor httpContextAccessor,
    ILogger<NeighborService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<IEnumerable<Neighbor>> GetNeighborsAsync(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync(ApiRoutes.Neighbors.GetAll);
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

        return Array.Empty<Neighbor>();
    }
}