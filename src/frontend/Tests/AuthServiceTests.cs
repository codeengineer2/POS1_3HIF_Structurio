using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Structurio.Classes;
using Structurio.Interfaces;
using Structurio.Services;

namespace Tests
{
    
    [TestClass]
    public class AuthServiceTests
    {
        [TestMethod]
        public async Task Login_User_Valid()
        {
            // Arrange
            var expectedUser = new User { Id = 1, Firstname = "Max" };

            var mockApi = new Mock<IApiService>();
            mockApi.Setup(api => api.LoginAsync("test@test.com", "test"))
                .ReturnsAsync(new LoginResponse
                {
                    Success = true,
                    User = expectedUser
                });

            var authService = new AuthService(mockApi.Object);

            // Act
            var result = await authService.TryLogin("test@test.com", "test");

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(authService.CurrentUser);
            Assert.AreEqual(expectedUser.Id, authService.CurrentUser.Id);
        }

        [TestMethod]
        public async Task Login_User_Invalid()
        {
            // Arrange
            var mockApi = new Mock<IApiService>();
            mockApi.Setup(api => api.LoginAsync("wrong", "wrong")).ReturnsAsync((LoginResponse?)null);

            var authService = new AuthService(mockApi.Object);

            // Act
            var result = await authService.TryLogin("wrong", "wrong");

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(authService.CurrentUser);
        }

        [TestMethod]
        public async Task Register_User()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@test.com", Password = "test" };

            var mockApi = new Mock<IApiService>();
            mockApi.Setup(api => api.RegisterAsync(request)).ReturnsAsync(true);

            var authService = new AuthService(mockApi.Object);

            // Act
            var result = await authService.Register(request);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Logout_User()
        {
            // Arrange
            var mockApi = new Mock<IApiService>();
            var authService = new AuthService(mockApi.Object);
            typeof(AuthService).GetProperty("CurrentUser")!.SetValue(authService, new User { Id = 42 });

            // Act
            authService.Logout();

            // Assert
            Assert.IsNull(authService.CurrentUser);
        }
    }
}