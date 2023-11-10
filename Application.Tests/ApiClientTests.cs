using Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests
{
    public class ApiClientTests
    {
        private ApiClient _apiClient;
        private HttpClient _httpClient;
        private Mock<ILogger<ApiClient>> _mockLogger = new();
        private Mock<HttpMessageHandler> _mockHttpHandler = new();

        public ApiClientTests()
        {
            _httpClient = new HttpClient(_mockHttpHandler.Object);
            _apiClient = new ApiClient(_httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task Test_GetAsync_SuccessResponse()
        {
            var mockResponse = new HttpResponseMessage { Content = new StringContent("ok computer"), StatusCode = System.Net.HttpStatusCode.Accepted };
            _mockHttpHandler.Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(mockResponse)
           .Verifiable();

            var response = await _apiClient.GetAsync("https://www.example.com");
            Assert.NotNull(response);
            _mockLogger.Verify(l => l.Log(
                LogLevel.Trace,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("responseStatusCode")),
                null,
                null), Times.Once);
        }

        [Fact]
        public async Task Test_GetAsync_FailResponse()
        {
            var mockResponse = new HttpResponseMessage { Content = new StringContent("fail"), StatusCode = System.Net.HttpStatusCode.BadRequest };
            _mockHttpHandler.Protected()
           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(mockResponse)
           .Verifiable();

            await Assert.ThrowsAnyAsync<Exception>(() => _apiClient.GetAsync("https://www.example.com"));
            _mockLogger.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("fail")),
                It.IsAny<Exception>(),
                null), Times.Once);
        }
    }
}
