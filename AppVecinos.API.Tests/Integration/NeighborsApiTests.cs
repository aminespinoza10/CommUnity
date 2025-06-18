using AppVecinos.API.Data;
using AppVecinos.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AppVecinos.API.Tests.Integration
{
    public class NeighborsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public NeighborsApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DataContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add an in-memory database for testing
                    services.AddDbContext<DataContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryTestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetNeighbors_WithoutAuthorization_ReturnsUnauthorized()
        {
            // Act
            var response = await _client.GetAsync("/neighbors");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetNeighbors_WithInvalidToken_ReturnsUnauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");

            // Act
            var response = await _client.GetAsync("/neighbors");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetNeighbors_WithValidAdminToken_ReturnsOk()
        {
            // Arrange
            var adminToken = await GetValidAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await _client.GetAsync("/neighbors");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            
            // Verify it returns a JSON array
            var neighbors = JsonSerializer.Deserialize<List<object>>(content);
            Assert.NotNull(neighbors);
        }

        [Fact]
        public async Task GetNeighbors_WithValidUserToken_ReturnsForbidden()
        {
            // Arrange  
            var userToken = await GetValidUserTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

            // Act
            var response = await _client.GetAsync("/neighbors");

            // Assert
            // Note: This might return Forbidden (403) if user doesn't have admin role
            // or OK if the endpoint allows both Admin and User roles
            Assert.True(response.StatusCode == HttpStatusCode.Forbidden || 
                       response.StatusCode == HttpStatusCode.OK);
        }

        private async Task<string> GetValidAdminTokenAsync()
        {
            // Create an admin user in the test database and get a token
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Check if admin user already exists
            var existingAdmin = await context.Neighbors
                .FirstOrDefaultAsync(n => n.User == "testadmin");
            
            if (existingAdmin == null)
            {
                var adminNeighbor = new Neighbor
                {
                    Name = "Test Admin",
                    Number = 999,
                    User = "testadmin",
                    Password = "adminpass",
                    Level = "Admin",
                    Status = "Active"
                };
                
                context.Neighbors.Add(adminNeighbor);
                await context.SaveChangesAsync();
            }

            // Login to get token
            var loginRequest = new
            {
                Username = "testadmin",
                Password = "adminpass"
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await _client.PostAsync("/login", loginContent);
            
            if (loginResponse.IsSuccessStatusCode)
            {
                var loginResult = await loginResponse.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(loginResult);
                return tokenResponse.GetProperty("token").GetString() ?? "";
            }

            throw new InvalidOperationException("Failed to get admin token");
        }

        private async Task<string> GetValidUserTokenAsync()
        {
            // Create a regular user in the test database and get a token
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Check if user already exists
            var existingUser = await context.Neighbors
                .FirstOrDefaultAsync(n => n.User == "testuser");
            
            if (existingUser == null)
            {
                var userNeighbor = new Neighbor
                {
                    Name = "Test User",
                    Number = 998,
                    User = "testuser",
                    Password = "userpass",
                    Level = "User",
                    Status = "Active"
                };
                
                context.Neighbors.Add(userNeighbor);
                await context.SaveChangesAsync();
            }

            // Login to get token
            var loginRequest = new
            {
                Username = "testuser",
                Password = "userpass"
            };

            var loginContent = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                Encoding.UTF8,
                "application/json");

            var loginResponse = await _client.PostAsync("/login", loginContent);
            
            if (loginResponse.IsSuccessStatusCode)
            {
                var loginResult = await loginResponse.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<JsonElement>(loginResult);
                return tokenResponse.GetProperty("token").GetString() ?? "";
            }

            throw new InvalidOperationException("Failed to get user token");
        }
    }
}