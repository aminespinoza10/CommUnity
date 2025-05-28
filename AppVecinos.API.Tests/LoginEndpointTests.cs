using System.Net;
using System.Net.Http.Json;
using System.Text;
using AppVecinos.API.Extensions;
using AppVecinos.API.Models;
using AppVecinos.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace AppVecinos.API.Tests
{
    public class LoginEndpointTests
    {
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var mockNeighborService = new Mock<INeighborService>();
            var testNeighbor = new Neighbor
            {
                Id = 1,
                Name = "Test User",
                Number = 123,
                User = "testuser",
                Password = "password",
                Level = "Admin",
                Status = "Active"
            };

            mockNeighborService
                .Setup(service => service.GetNeighborByCredentialsAsync("testuser", "password"))
                .ReturnsAsync(testNeighbor);

            // Create a mock configuration with JWT settings
            var inMemorySettings = new Dictionary<string, string> {
                {"JwtSettings:SecretKey", "TestSecretKeyWithAtLeast32Characters1234567890"},
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Create a test server with the application
            var webHostBuilder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddScoped<INeighborService>(_ => mockNeighborService.Object);
                    services.AddJwtAuthenticationAndAuthorization(configuration);
                    services.AddEndpointsApiExplorer();
                })
                .UseStartup<TestStartup>();

            var testServer = new TestServer(webHostBuilder);
            var client = testServer.CreateClient();

            // Act
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password"
            };

            var response = await client.PostAsJsonAsync("/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Token", responseContent);

            // Verify service was called with correct parameters
            mockNeighborService.Verify(
                service => service.GetNeighborByCredentialsAsync("testuser", "password"),
                Times.Once);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var mockNeighborService = new Mock<INeighborService>();

            // Setup to return null for invalid credentials
            mockNeighborService
                .Setup(service => service.GetNeighborByCredentialsAsync("testuser", "wrongpassword"))
                .ReturnsAsync((Neighbor)null);

            // Create a mock configuration with JWT settings
            var inMemorySettings = new Dictionary<string, string> {
                {"JwtSettings:SecretKey", "TestSecretKeyWithAtLeast32Characters1234567890"},
                {"JwtSettings:Issuer", "TestIssuer"},
                {"JwtSettings:Audience", "TestAudience"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Create a test server with the application
            var webHostBuilder = new WebHostBuilder()
                .UseConfiguration(configuration)
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IConfiguration>(configuration);
                    services.AddScoped<INeighborService>(_ => mockNeighborService.Object);
                    services.AddJwtAuthenticationAndAuthorization(configuration);
                    services.AddEndpointsApiExplorer();
                })
                .UseStartup<TestStartup>();

            var testServer = new TestServer(webHostBuilder);
            var client = testServer.CreateClient();

            // Act
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            var response = await client.PostAsJsonAsync("/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            // Verify service was called with correct parameters
            mockNeighborService.Verify(
                service => service.GetNeighborByCredentialsAsync("testuser", "wrongpassword"),
                Times.Once);
        }
    }

    // Create a minimal startup class for the test server
    public class TestStartup
    {
        public IConfiguration Configuration { get; }

        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Setup login endpoint for testing
                endpoints.MapPost("/login", async (INeighborService service, LoginRequest loginRequest, IConfiguration configuration) =>
                {
                    var result = await service.GetNeighborByCredentialsAsync(loginRequest.Username, loginRequest.Password);
                    if (result != null)
                    {
                        var token = AuthenticationServiceExtensions.GenerateJwtToken(loginRequest.Username, result.Level, configuration);
                        return Results.Ok(new { Token = token });
                    }
                    return Results.Unauthorized();
                });
            });
        }
    }
}