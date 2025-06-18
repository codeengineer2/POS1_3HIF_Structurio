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
    public class TimestampHttpClientsTest
    {
        /// <summary>
        /// Tests for Timestamp HTTP client methods (Get, Post, Put)
        /// </summary>
        [TestClass]
        public class TimestampHttpClientTests
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
            public async Task GetTimestampAsync_Success()
            {
                // Arrange
                var sample = new List<Timestamp_Json>
            {
                new Timestamp_Json { zid = 3, uid = 1, pid = 0, datum_in = "2025-06-18", checkin = "08:00:00", datum_out = "2025-06-18", checkout = "17:00:00", duration = "09:00" }
            };
                string jsonTs = JsonSerializer.Serialize(sample);
                SetupResponse(HttpStatusCode.OK, jsonTs);

                // Act
                var result = await Get_timestamp.GetAsync(httpClient, uid: 1);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(3, result[0].zid);
                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.ToString().EndsWith("/timestamps/1")),
                    ItExpr.IsAny<CancellationToken>());
            }

            [TestMethod]
            [ExpectedException(typeof(HttpRequestException))]
            public async Task GetTimestampAsync_Error_Throws()
            {
                // Arrange
                SetupException();

                // Act
                await Get_timestamp.GetAsync(httpClient, uid: 1);
            }

            [TestMethod]
            public async Task PostTimestampAsync_Success()
            {
                // Arrange
                var now = DateTime.Now;
                var created = new Timestamp_Json { zid = 7, uid = 1, pid = 0, datum_in = now.ToString("yyyy-MM-dd"), checkin = now.ToString("HH:mm:ss"), datum_out = "", checkout = "", duration = "00:00" };
                string respTs = JsonSerializer.Serialize(created);
                SetupResponse(HttpStatusCode.OK, respTs);

                // Act
                var result = await Post_timestamp.CreateAsync(httpClient, uid: 1, checkIn: now);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(7, result.zid);
                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post && req.RequestUri!.ToString().EndsWith("/timestamps")),
                    ItExpr.IsAny<CancellationToken>());
            }

            [TestMethod]
            [ExpectedException(typeof(HttpRequestException))]
            public async Task PostTimestampAsync_Error_Throws()
            {
                // Arrange
                SetupException();

                // Act
                await Post_timestamp.CreateAsync(httpClient, uid: 1, checkIn: DateTime.Now);
            }

            [TestMethod]
            public async Task PutTimestampAsync_Success()
            {
                // Arrange
                SetupResponse(HttpStatusCode.OK, "{}");
                var checkIn = new DateTime(2025, 6, 18, 8, 0, 0);
                var checkOut = new DateTime(2025, 6, 18, 17, 0, 0);
                var duration = "09:00";

                // Act
                await Put_timestamp.UpdateAsync(httpClient, uid: 1, zid: 5, checkIn: checkIn, checkOut: checkOut, duration: duration);

                // Assert
                handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Put && req.RequestUri!.ToString().EndsWith($"/timestamps/1/5")),
                    ItExpr.IsAny<CancellationToken>());
            }

            [TestMethod]
            [ExpectedException(typeof(HttpRequestException))]
            public async Task PutTimestampAsync_Error_Throws()
            {
                // Arrange
                SetupResponse(HttpStatusCode.BadRequest, "");

                // Act
                await Put_timestamp.UpdateAsync(httpClient, uid: 1, zid: 1, checkIn: DateTime.Now, checkOut: DateTime.Now, duration: "00:00");
            }
        }
    }
}
