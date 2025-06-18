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
        private Mock<IApiService> mockApiService = null!;
        private AuthService authService = null!;

        [TestInitialize]
        public void Setup()
        {
            mockApiService = new Mock<IApiService>();
            authService = new AuthService(mockApiService.Object);
        }

        [TestMethod]
        public async Task TryLogin_Valid()
        {
            // Arrange
            var testUser = new User { Id = 1, Email = "test@test.com" };
            var loginResponse = new LoginResponse { Success = true, User = testUser };
            mockApiService.Setup(api => api.LoginAsync("test@test.com", "Test1995+")).ReturnsAsync(loginResponse);

            // Act
            var result = await authService.TryLogin("test@test.com", "Test1995+");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(testUser, authService.CurrentUser);
        }

        [TestMethod]
        public async Task TryLogin_Login_Invalid()
        {
            // Arrange
            mockApiService.Setup(api => api.LoginAsync("test@test.com", "Test1995+")).ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await authService.TryLogin("test@test.com", "Test1995+");

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(authService.CurrentUser);
        }

        [TestMethod]
        public async Task TryLogin_Overall_Invalid()
        {
            // Arrange
            var loginResponse = new LoginResponse { Success = false, User = null };
            mockApiService.Setup(api => api.LoginAsync("test@example.com", "Test1995+")).ReturnsAsync(loginResponse);

            // Act
            var result = await authService.TryLogin("test@test.com", "Test1995+");

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(authService.CurrentUser);
        }

        [TestMethod]
        public async Task Register_Invalid()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@test.com", Password = "Test1995+" };
            mockApiService.Setup(api => api.RegisterAsync(request)).ReturnsAsync(true);

            // Act
            var result = await authService.Register(request);

            // Assert
            Assert.IsTrue(result);
            mockApiService.Verify(api => api.RegisterAsync(request), Times.Once);
        }

        [TestMethod]
        public async Task Register_Valid()
        {
            // Arrange
            var request = new RegisterRequest { Email = "test@test.com", Password = "Test1995+" };
            mockApiService.Setup(api => api.RegisterAsync(request)).ReturnsAsync(false);

            // Act
            var result = await authService.Register(request);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Logout_User()
        {
            // Arrange
            var testUser = new User { Id = 1, Email = "test@test.com" };
            typeof(AuthService).GetProperty(nameof(AuthService.CurrentUser))!.SetValue(authService, testUser);

            // Act
            authService.Logout();

            // Assert
            Assert.IsNull(authService.CurrentUser);
        }
    }
}