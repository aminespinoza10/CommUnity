using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        /*var loginRequest = new
        {
            Username = username,
            Password = password
        };

        var jsonContent = new StringContent(
            JsonSerializer.Serialize(loginRequest),
            Encoding.UTF8,
            "application/json");       */

        var response = await _httpClient.PostAsJsonAsync(ApiRoutes.Auth.Login, new { username, password });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
        
        /*var response = await _httpClient.PostAsync(ApiRoutes.Auth.Login, jsonContent);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody; // Return the token if the login is successful.
        }

        return null; // Return null if login fails. */
    }
}
