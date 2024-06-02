using AuthenticationService.Controllers;
using AuthenticationService.Data.Entities;
using AuthenticationService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace AuthenticationService.Tests
{
    public class AuthenticationControllerTests
    {
        [Fact]
        public async Task Token_ValidCredentials_ReturnsJwtToken()
        {
            using (var dbContext = ContextGenerator.Generate())
            {
                dbContext.Users.Add(new User { Id = 1, Username = "test", Password = "password", Role = "user" });
                await dbContext.SaveChangesAsync();

                var mockCache = new Mock<IDistributedCache>();
                var controller = new AuthenticationController(dbContext, mockCache.Object);
                var userModel = new UserModel { Username = "test", Password = "password" };

                var result = await controller.Token(userModel);

                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.NotNull(okResult.Value);
                Assert.True(okResult.Value.ToString().Contains("JwtToken"));
            }
        }

        [Fact]
        public async Task Token_InvalidCredentials_ReturnsUnauthorized()
        {
            using (var dbContext = ContextGenerator.Generate())
            {
                dbContext.Users.Add(new User { Id = 1, Username = "test", Password = "password", Role = "user" });
                await dbContext.SaveChangesAsync();

                var mockCache = new Mock<IDistributedCache>();
                var controller = new AuthenticationController(dbContext, mockCache.Object);
                var userModel = new UserModel { Username = "invaliduser", Password = "invalidpassword" };

                var result = await controller.Token(userModel);

                Assert.IsType<UnauthorizedResult>(result);
            }
        }

        [Fact]
        public async Task SignUp_ValidModel_ReturnsOk()
        {
            var userModel = new UserModel { Username = "newuser", Password = "newpassword" };

            using (var dbContext = ContextGenerator.Generate())
            {
                var mockCache = new Mock<IDistributedCache>();
                var controller = new AuthenticationController(dbContext, mockCache.Object);

                // Act
                var result = await controller.SignUp(userModel);

                // Assert
                Assert.IsType<OkResult>(result);
            }
        }

        [Fact]
        public async Task SignUp_ModelAlreadyExists_ReturnsBadRequest()
        {
            var userModel = new UserModel { Username = "existinguser", Password = "existingpassword" };

            using (var dbContext = ContextGenerator.Generate())
            {
                dbContext.Users.Add(new User { Id = 1, Username = "existinguser", Password = "existingpassword", Role = "user" });
                await dbContext.SaveChangesAsync();

                var mockCache = new Mock<IDistributedCache>();
                var controller = new AuthenticationController(dbContext, mockCache.Object);

                var result = await controller.SignUp(userModel);

                var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
                Assert.Equal("This model already exists.", badRequestResult.Value);
            }
        }
    }
}