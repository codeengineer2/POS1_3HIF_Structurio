using Moq;
using Moq.Protected;
using Structurio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
namespace Tests
{
    /// <summary>
    /// Tests for Abrechnung HTTP client methods (Get and Post)
    /// </summary>
    [TestClass]
    public class AbrechnungHttpClientTests
    {
        private Mock<HttpMessageHandler> handlerMock;
        private HttpClient httpClient;

        [TestInitialize]
        public void Setup()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };
        }

        private void SetupResponse(HttpStatusCode statusCode, string content)
        {
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                })
                .Verifiable();
        }

        private void SetupException()
        {
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Mock exception"))
                .Verifiable();
        }

        [TestMethod]
        public async Task GetAbrechnungAsync_Success()
        {
            // Arrange
            var sample = new List<Abrechnung_JSON>
            {
                new Abrechnung_JSON { Aid = 5, Uid = 1, Pid = 2, Name = "Test", Date = DateTime.Today, Price = 99.9, Category = "Cat", Rechnung = null }
            };
            string json = JsonSerializer.Serialize(sample);
            SetupResponse(HttpStatusCode.OK, json);

            // Act
            var result = await Get_Abrechnung.GetAsync(httpClient, uid: 1, pid: 2);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(5, result[0].Aid);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.ToString().EndsWith("/abrechnung/1/2")),
                ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetAbrechnungAsync_ErrorStatus_Throws()
        {
            // Arrange
            SetupResponse(HttpStatusCode.BadRequest, "");

            // Act
            await Get_Abrechnung.GetAsync(httpClient, uid: 1, pid: 1);
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task GetAbrechnungAsync_Exception_Throws()
        {
            // Arrange
            SetupException();

            // Act
            await Get_Abrechnung.GetAsync(httpClient, uid: 1, pid: 1);
        }

        [TestMethod]
        public async Task PostAbrechnungAsync_Success()
        {
            // Arrange
            var created = new Abrechnung_JSON { Aid = 10, Uid = 1, Pid = 2, Name = "New", Date = DateTime.Today, Price = 10.0, Category = "Cat", Rechnung = "file.pdf" };
            var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            string resp = JsonSerializer.Serialize(created, opts);
            SetupResponse(HttpStatusCode.OK, resp);

            // Act
            var result = await Post_Abrechnung.CreateAsync(httpClient, uid: 1, pid: 2, name: "New", date: created.Date, price: created.Price, category: created.Category, rechnungPath: created.Rechnung!);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(10, result.Aid);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri!.ToString().EndsWith("/abrechnung")),
                ItExpr.IsAny<CancellationToken>());
        }

        [TestMethod]
        [ExpectedException(typeof(HttpRequestException))]
        public async Task PostAbrechnungAsync_Error_Throws()
        {
            // Arrange
            SetupResponse(HttpStatusCode.InternalServerError, "");

            // Act
            await Post_Abrechnung.CreateAsync(httpClient, uid: 1, pid: 1, name: "", date: DateTime.Now, price: 0, category: "", rechnungPath: "");
        }
    }
}
