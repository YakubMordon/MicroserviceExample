using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using PaymentService.Controllers;
using PaymentService.Data.Entities;
using PaymentService.Models;

namespace PaymentService.Tests
{
    public class PaymentControllerTests
    {
        [Fact]
        public async Task ProcessPaymentAsync_ValidRequest_ReturnsOkResult()
        {
            using (var dbContext = ContextGenerator.Generate())
            {
                var orderMocked = new Order
                    { CarId = 1, Id = 1, IsCompleted = false, OrderDate = DateTime.Today, UserId = 1 };

                dbContext.Add(orderMocked);

                await dbContext.SaveChangesAsync();

                var mockCache = new Mock<IDistributedCache>();

                var controller = new PaymentController(dbContext, mockCache.Object);
                var paymentRequest = new PaymentModel { Token = "valid-token", OrderId = 1, Amount = 100 };

                // Act
                var result = await controller.ProcessPaymentAsync(paymentRequest);

                // Assert
                Assert.IsType<OkObjectResult>(result);
                var okResult = result as OkObjectResult;
                Assert.Equal("Payment processed successfully.", okResult.Value);
                Assert.Equal(1, dbContext.Payments.Count());
            }
        }

        [Fact]
        public async Task ProcessPaymentAsync_InvalidRequest_ReturnsBadRequestResult()
        {
            using (var dbContext = ContextGenerator.Generate())
            {
                var mockCache = new Mock<IDistributedCache>();

                var controller = new PaymentController(dbContext, mockCache.Object);
                var paymentRequest = new PaymentModel { Token = "valid-token", OrderId = 99, Amount = 100 };

                // Act
                var result = await controller.ProcessPaymentAsync(paymentRequest);

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
                var badRequestResult = result as BadRequestObjectResult;
                Assert.Equal("Order doesn't exist", badRequestResult.Value);
            }
        }
    }
}