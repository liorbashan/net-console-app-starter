//using Application.Models.Configuration;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Moq;
//using Moq.Protected;
//using System;
//using System.Net.Http;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace Application.Tests
//{
//    public class AppTests
//    {
//        private Mock<HttpMessageHandler> _mockHttpHandler = new();
//        private HttpClient _httpClient;
//        private Mock<IOptions<ApplicationSettings>> _mockConfig = new();
//        private ApplicationSettings _appSettings;
//        private Mock<ILogger<App>> _mockLogger = new Mock<ILogger<App>>();
//        private App _app;

//        public AppTests()
//        {
//            _appSettings = new ApplicationSettings()
//            {
//                BaseUrl = "https://example.com",
//                BearerToken = "fakeToken"
//            };
//            _mockConfig.Setup(ap => ap.Value).Returns(_appSettings);
//            _httpClient = new HttpClient(_mockHttpHandler.Object);
//            _app = new App(_httpClient, _mockLogger.Object, _mockConfig.Object);
//        }
//        [Fact]
//        public async Task SuccesfullHttpResponse()
//        {
//            var mockResponse = new HttpResponseMessage { Content = new StringContent("ok computer"), StatusCode = System.Net.HttpStatusCode.Accepted };
//            _mockHttpHandler.Protected()
//           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//           .ReturnsAsync(mockResponse)
//           .Verifiable();

//            await _app.Run();
//            _mockLogger.Verify(x => x.Log(LogLevel.Information,
//                It.IsAny<EventId>(),
//                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(" ")),
//                It.IsAny<Exception>(),
//                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

//            _mockHttpHandler.Protected().Verify(
//            "SendAsync",
//            Times.Exactly(1),
//            ItExpr.Is<HttpRequestMessage>(req =>
//                req.Method == HttpMethod.Get
//                && req.RequestUri.ToString().Equals($"{_appSettings.BaseUrl}/v3.1/name/deutschland")
//            ),
//            ItExpr.IsAny<CancellationToken>());
//        }

//        [Fact]
//        public async Task Test_HttpResponseStatusCodeIsNotSuccess_ShouldThrowException()
//        {
//            var mockResponse = new HttpResponseMessage { Content = new StringContent("bad request"), StatusCode = System.Net.HttpStatusCode.BadRequest };
//            _mockHttpHandler.Protected()
//           .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//           .ReturnsAsync(mockResponse);

//            await Assert.ThrowsAsync<Exception>(_app.Run);
//            _mockLogger.Verify(x => x.Log(LogLevel.Error,
//                It.IsAny<EventId>(),
//                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("http failed")),
//                It.IsAny<Exception>(),
//                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
//        }
//    }
//}