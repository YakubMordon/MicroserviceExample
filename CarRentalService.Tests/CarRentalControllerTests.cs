using CarRentalService.Controllers;
using CarRentalService.Data.Entities;
using CarRentalService.Data;
using CarRentalService.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace CarRentalService.Tests
{
    public class CarRentalControllerTests
    {
        private readonly Mock<IDistributedCache> _cacheMock = new Mock<IDistributedCache>();
        private readonly Mock<ICarRentalDbContext> _dbContextMock = new Mock<ICarRentalDbContext>();

        [Fact]
        public async Task GetAvailableCarsAsync_ReturnsOkObjectResultWithListOfCars()
        {
            // Arrange
            var controller = new CarRentalController(_cacheMock.Object, _dbContextMock.Object);
            var expectedCars = new List<Car>
            {
                new Car { Id = 1, Brand = "Toyota", Model = "Camry", IsAvailable = true },
                new Car { Id = 2, Brand = "Honda", Model = "Accord", IsAvailable = true }
            };

            _dbContextMock.Setup(db => db.GetAvailableCarsFromDbAsync()).ReturnsAsync(expectedCars);

            // Act
            var result = await controller.GetAvailableCarsAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualCars = Assert.IsAssignableFrom<IEnumerable<Car>>(okResult.Value);
            Assert.Equal(expectedCars, actualCars);
        }

        [Fact]
        public async Task OrderCarAsync_WithValidRequest_ReturnsOkObjectResult()
        {
            // Arrange
            var controller = new CarRentalController(_cacheMock.Object, _dbContextMock.Object);
            var orderModel = new OrderModel { CarId = 1, UserId = 1, Token = "valid-token" };

            _dbContextMock.Setup(db => db.OrderCarInDbAsync(orderModel)).ReturnsAsync(true);

            // Act
            var result = await controller.OrderCarAsync(orderModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Car successfully ordered.", okResult.Value);
        }

        [Fact]
        public async Task ReturnCarAsync_WithValidRequest_ReturnsOkObjectResult()
        {
            // Arrange
            var controller = new CarRentalController(_cacheMock.Object, _dbContextMock.Object);
            var returnModel = new ReturnModel { OrderId = 1, IsDamaged = false, Token = "valid-token" };

            _dbContextMock.Setup(db => db.ReturnCarFromDbAsync(returnModel)).ReturnsAsync(true);

            // Act
            var result = await controller.ReturnCarAsync(returnModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Car successfully returned.", okResult.Value);
        }

        [Fact]
        public async Task GetRentedCarsAsync_WithValidToken_ReturnsOkObjectResultWithListOfCars()
        {
            // Arrange
            var controller = new CarRentalController(_cacheMock.Object, _dbContextMock.Object);
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0MWYxZGJiNC0wZmUyLTQ1ZTgtOTk3Ny0wOTk1ZTcxNjEzYjAiLCJzdWIiOiJhZG1pbiIsInVzZXJpZCI6IjEiLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE3MTA1OTI2MTAsImV4cCI6MTcxMDU5NjIxMCwiaWF0IjoxNzEwNTkyNjEwLCJpc3MiOiJodHRwczovL2F1dGhlbnRpY2F0aW9uc2VydmljZTo4MDg0IiwiYXVkIjoiaHR0cHM6Ly9jYXJyZW50YWxzZXJ2aWNlOjgwODMgLCBodHRwczovL3BheW1lbnRzZXJ2aWNlOjgwODEsIGh0dHBzOi8vY29tcGxleGxhYmdhdGV3YXk6ODA4MiJ9.6YvAMo_OzHXJ8a28XuNfXGcl5b5aOYupgcpGK0JW2yQ";
            var expectedCars = new List<Car>
            {
                new Car { Id = 1, Brand = "Toyota", Model = "Camry" },
                new Car { Id = 2, Brand = "Honda", Model = "Accord" }
            };

            _dbContextMock.Setup(db => db.GetRentedCarsFromDbAsync()).ReturnsAsync(expectedCars);

            // Act
            var result = await controller.GetRentedCarsAsync(token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualCars = Assert.IsAssignableFrom<IEnumerable<Car>>(okResult.Value);
            Assert.Equal(expectedCars, actualCars);
        }

        [Fact]
        public async Task GetRentalHistoryAsync_WithValidTokenAndAdminRole_ReturnsOkObjectResultWithRentalHistory()
        {
            // Arrange
            var controller = new CarRentalController(_cacheMock.Object, _dbContextMock.Object);
            var validToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0MWYxZGJiNC0wZmUyLTQ1ZTgtOTk3Ny0wOTk1ZTcxNjEzYjAiLCJzdWIiOiJhZG1pbiIsInVzZXJpZCI6IjEiLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE3MTA1OTI2MTAsImV4cCI6MTcxMDU5NjIxMCwiaWF0IjoxNzEwNTkyNjEwLCJpc3MiOiJodHRwczovL2F1dGhlbnRpY2F0aW9uc2VydmljZTo4MDg0IiwiYXVkIjoiaHR0cHM6Ly9jYXJyZW50YWxzZXJ2aWNlOjgwODMgLCBodHRwczovL3BheW1lbnRzZXJ2aWNlOjgwODEsIGh0dHBzOi8vY29tcGxleGxhYmdhdGV3YXk6ODA4MiJ9.6YvAMo_OzHXJ8a28XuNfXGcl5b5aOYupgcpGK0JW2yQ";
            var rentalHistory = new List<Order>
            {
                new Order { Id = 1, CarId = 1, UserId = 1, OrderDate = DateTime.Now, IsCompleted = true },
                new Order { Id = 2, CarId = 2, UserId = 2, OrderDate = DateTime.Now, IsCompleted = false }
            };

            _dbContextMock.Setup(db => db.GetRentalHistoryFromDbAsync()).ReturnsAsync(rentalHistory);

            // Act
            var result = await controller.GetRentalHistoryAsync(validToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualRentalHistory = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
            Assert.Equal(rentalHistory, actualRentalHistory);
        }

        [Fact]
        public async Task GetDamagedCarsAsync_WithValidTokenAndAdminRole_ReturnsOkObjectResultWithDamagedCars()
        {
            // Arrange
            var controller = new CarRentalController(_cacheMock.Object, _dbContextMock.Object);
            var validToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0MWYxZGJiNC0wZmUyLTQ1ZTgtOTk3Ny0wOTk1ZTcxNjEzYjAiLCJzdWIiOiJhZG1pbiIsInVzZXJpZCI6IjEiLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE3MTA1OTI2MTAsImV4cCI6MTcxMDU5NjIxMCwiaWF0IjoxNzEwNTkyNjEwLCJpc3MiOiJodHRwczovL2F1dGhlbnRpY2F0aW9uc2VydmljZTo4MDg0IiwiYXVkIjoiaHR0cHM6Ly9jYXJyZW50YWxzZXJ2aWNlOjgwODMgLCBodHRwczovL3BheW1lbnRzZXJ2aWNlOjgwODEsIGh0dHBzOi8vY29tcGxleGxhYmdhdGV3YXk6ODA4MiJ9.6YvAMo_OzHXJ8a28XuNfXGcl5b5aOYupgcpGK0JW2yQ";
            var damagedCars = new List<Car>
            {
                new Car { Id = 1 },
                new Car { Id = 2 }
            };

            _dbContextMock.Setup(db => db.GetDamagedCarsFromDbAsync()).ReturnsAsync(damagedCars);

            // Act
            var result = await controller.GetDamagedCarsAsync(validToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualDamagedCars = Assert.IsAssignableFrom<IEnumerable<Car>>(okResult.Value);
            Assert.Equal(damagedCars, actualDamagedCars);
        }


        [Fact]
        public async Task RejectOrderAsync_WithValidOrderIdAndToken_ReturnsOkObjectResult()
        {
            // Arrange
            var controller = new CarRentalController(_cacheMock.Object, _dbContextMock.Object);
            var orderId = 1;
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiI0MWYxZGJiNC0wZmUyLTQ1ZTgtOTk3Ny0wOTk1ZTcxNjEzYjAiLCJzdWIiOiJhZG1pbiIsInVzZXJpZCI6IjEiLCJyb2xlIjoiYWRtaW4iLCJuYmYiOjE3MTA1OTI2MTAsImV4cCI6MTcxMDU5NjIxMCwiaWF0IjoxNzEwNTkyNjEwLCJpc3MiOiJodHRwczovL2F1dGhlbnRpY2F0aW9uc2VydmljZTo4MDg0IiwiYXVkIjoiaHR0cHM6Ly9jYXJyZW50YWxzZXJ2aWNlOjgwODMgLCBodHRwczovL3BheW1lbnRzZXJ2aWNlOjgwODEsIGh0dHBzOi8vY29tcGxleGxhYmdhdGV3YXk6ODA4MiJ9.6YvAMo_OzHXJ8a28XuNfXGcl5b5aOYupgcpGK0JW2yQ";

            _dbContextMock.Setup(db => db.RejectOrderFromDbAsync(orderId)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.RejectOrderAsync(orderId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Order rejected successfully.", okResult.Value);
        }

    }
}