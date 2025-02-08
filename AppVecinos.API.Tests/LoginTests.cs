using Xunit;
using Moq;
using AppVecinos.API;
using AppVecinos.API.Services;
using AppVecinos.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AppVecinos.API.Tests
{
    public class LoginTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public LoginTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_ReturnsOk_WithValidCredentials()
        {
            // Arrange
            var client = _factory.CreateClient();
            var mockService = new Mock<INeighborService>();
            mockService.Setup(service => service.GetNeighborByCredentialsAsync("Amin", "1234"))
                       .ReturnsAsync(new Neighbor { User = "Amin", Level = "Admin" });

            // Act
            var response = await client.PostAsJsonAsync("/login", new LoginRequest { username = "Amin", password = "1234" });

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Token", responseString);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WithInvalidCredentials()
        {
            // Arrange
            var client = _factory.CreateClient();
            var mockService = new Mock<INeighborService>();
            mockService.Setup(service => service.GetNeighborByCredentialsAsync("invalidUser", "invalidPassword"))
                       .ReturnsAsync((Neighbor)null);

            // Act
            var response = await client.PostAsJsonAsync("/login", new LoginRequest { Username = "invalidUser", Password = "invalidPassword" });

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}