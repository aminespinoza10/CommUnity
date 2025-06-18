using AppVecinos.API.Data;
using AppVecinos.API.Models;
using AppVecinos.API.Repositories;
using AppVecinos.API.Services;
using Moq;
using Xunit;

namespace AppVecinos.API.Tests.Services
{
    public class NeighborServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Neighbor>> _mockNeighborRepository;
        private readonly NeighborService _neighborService;

        public NeighborServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockNeighborRepository = new Mock<IGenericRepository<Neighbor>>();
            _mockUnitOfWork.Setup(x => x.NeighborRepository).Returns(_mockNeighborRepository.Object);
            _neighborService = new NeighborService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetNeighborsAsync_ReturnsAllNeighbors()
        {
            // Arrange
            var expectedNeighbors = new List<Neighbor>
            {
                new Neighbor 
                { 
                    Id = 1, 
                    Name = "John Doe", 
                    Number = 101, 
                    User = "johndoe", 
                    Password = "password123", 
                    Level = "User", 
                    Status = "Active" 
                },
                new Neighbor 
                { 
                    Id = 2, 
                    Name = "Jane Smith", 
                    Number = 102, 
                    User = "janesmith", 
                    Password = "password456", 
                    Level = "Admin", 
                    Status = "Active" 
                }
            };

            _mockNeighborRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedNeighbors);

            // Act
            var result = await _neighborService.GetNeighborsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(expectedNeighbors, result);
            _mockNeighborRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetNeighborsAsync_ReturnsEmptyList_WhenNoNeighborsExist()
        {
            // Arrange
            var emptyNeighborsList = new List<Neighbor>();

            _mockNeighborRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(emptyNeighborsList);

            // Act
            var result = await _neighborService.GetNeighborsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockNeighborRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetNeighborsAsync_CallsRepositoryOnce()
        {
            // Arrange
            var neighbors = new List<Neighbor>();
            _mockNeighborRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(neighbors);

            // Act
            await _neighborService.GetNeighborsAsync();

            // Assert
            _mockNeighborRepository.Verify(x => x.GetAllAsync(), Times.Once);
            _mockUnitOfWork.Verify(x => x.NeighborRepository, Times.Once);
        }
    }
}