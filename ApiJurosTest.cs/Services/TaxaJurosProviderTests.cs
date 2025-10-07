
using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApiJuros.Application.Services;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using System.Text.Json;
using ApiJuros.Application.DTOs;

namespace ApiJuros.Test.Services
{
    public class TaxaJurosProviderTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<ILogger<TaxaJurosProvider>> _loggerMock;
        private readonly TaxaJurosProvider _taxaJurosProvider;

        public TaxaJurosProviderTests()
        {
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _loggerMock = new Mock<ILogger<TaxaJurosProvider>>();
            _taxaJurosProvider = new TaxaJurosProvider(_httpClientFactoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_Should_Return_Correct_Rate_When_Api_Returns_Valid_Data()
        {
            // Arrange
            var bcbResponse = new[] { new BcbApiResponse("01/01/2023", 0.5m) };
            var jsonResponse = JsonSerializer.Serialize(bcbResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            };

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var result = await _taxaJurosProvider.GetTaxaJurosAtualAsync();

            // Assert
            result.Should().Be(0.5m);
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_Should_Throw_Exception_When_Api_Returns_Empty_Response()
        {
            // Arrange
            var bcbResponse = new BcbApiResponse[] { };
            var jsonResponse = JsonSerializer.Serialize(bcbResponse);
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            };

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _taxaJurosProvider.GetTaxaJurosAtualAsync());
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_Should_Throw_Exception_When_Api_Returns_Null_Response()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("null")
            };

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _taxaJurosProvider.GetTaxaJurosAtualAsync());
        }
    }
}
