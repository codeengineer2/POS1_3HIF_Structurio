using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Structurio.Classes;
using Structurio.Interfaces;
using Structurio.Services;

namespace Tests
{
    [TestClass]
    public class ApiServiceTests
    {
        private ApiService apiService = null!;
        private Mock<HttpMessageHandler> handlerMock = null!;

        [TestInitialize]
        public void Setup()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            var httpClient = new HttpClient(handlerMock.Object);
            apiService = new ApiService(httpClient);
        }

        private void SetupResponse(HttpStatusCode statusCode, string content = "")
        {
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            });
        }

        private void SetupException()
        {
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ThrowsAsync(new HttpRequestException("Mock Error!"));
        }

        [TestMethod]
        public async Task LoginAsync_Valid()
        {
            // Arrange
            var loginResponse = new LoginResponse { Success = true, User = new User { Id = 1 } };
            SetupResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(loginResponse));

            // Act
            var result = await apiService.LoginAsync("test@test.com", "test");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result!.Success);
        }

        [TestMethod]
        public async Task LoginAsync_Invalid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.Unauthorized);

            // Act
            var result = await apiService.LoginAsync("test@test.com", "test");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task LoginAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.LoginAsync("test@test.com", "test");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task RegisterAsync_Success()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK);

            // Act
            var result = await apiService.RegisterAsync(new RegisterRequest());

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task RegisterAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.RegisterAsync(new RegisterRequest());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task CreateProjectAsync_Valid()
        {
            // Arrange
            var response = new { pid = 1, board = new { id = 2, project_id = 1 } };
            SetupResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(response));

            // Act
            var result = await apiService.CreateProjectAsync(new ProjectRequest { Name = "Test", OwnerUid = 10 });

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result!.Id);
            Assert.AreEqual(2, result.Board.Id);
        }

        [TestMethod]
        public async Task CreateProjectAsync_Invalid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.BadRequest);

            // Act
            var result = await apiService.CreateProjectAsync(new ProjectRequest());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateProjectAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.CreateProjectAsync(new ProjectRequest());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddIssueAsync_Success()
        {
            // Arrange
            var response = new { issue_id = 50 };
            SetupResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(response));

            // Act
            var result = await apiService.AddIssueAsync(new AddIssueRequest { ColumnId = 1, Description = "Test" });

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(50, result!.Id);
        }

        [TestMethod]
        public async Task AddIssueAsync_Overall_Invalid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.BadRequest);

            // Act
            var result = await apiService.AddIssueAsync(new AddIssueRequest());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddIssueAsync_Data_Invalid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK, "{}");

            // Act
            var result = await apiService.AddIssueAsync(new AddIssueRequest());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddIssueAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.AddIssueAsync(new AddIssueRequest());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateIssueAsync_Valid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK);

            // Act
            var result = await apiService.UpdateIssueAsync(new UpdateIssueRequest());

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdateIssueAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.UpdateIssueAsync(new UpdateIssueRequest());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteIssueAsync_Valid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK);

            // Act
            var result = await apiService.DeleteIssueAsync(1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteIssueAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.DeleteIssueAsync(1);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AddColumnAsync_Valid()
        {
            // Arrange
            var response = new { cid = 7, name = "Spalte" };
            SetupResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(response));

            // Act
            var result = await apiService.AddColumnAsync(new AddColumnRequest());

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(7, result!.Id);
            Assert.AreEqual("Spalte", result.Name);
        }

        [TestMethod]
        public async Task AddColumnAsync_Invalid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK, "{}");

            // Act
            var result = await apiService.AddColumnAsync(new AddColumnRequest());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task AddColumnAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.AddColumnAsync(new AddColumnRequest());

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateColumnAsync_Valid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK);

            // Act
            var result = await apiService.UpdateColumnAsync(new UpdateColumnRequest());

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdateColumnAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.UpdateColumnAsync(new UpdateColumnRequest());

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteProjectAsync_Valid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK);

            // Act
            var result = await apiService.DeleteProjectAsync(1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteProjectAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.DeleteProjectAsync(1);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateProjectAsync_Valid()
        {
            // Arrange
            SetupResponse(HttpStatusCode.OK);

            // Act
            var result = await apiService.UpdateProjectAsync(new Project { Id = 1, Name = "P", Description = "Project", Color = "#FF5733" });

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task UpdateProjectAsync_Exception()
        {
            // Arrange
            SetupException();

            // Act
            var result = await apiService.UpdateProjectAsync(new Project());

            // Assert
            Assert.IsFalse(result);
        }
    }
}