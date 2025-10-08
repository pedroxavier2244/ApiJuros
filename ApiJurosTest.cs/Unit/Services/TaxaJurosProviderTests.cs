using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
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

        private void SetupHttpClient(HttpStatusCode statusCode, string content)
        {
            var httpResponseMessage = new HttpResponseMessage { StatusCode = statusCode, Content = new StringContent(content) };
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponseMessage);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            _httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_WhenApiReturnsValidData_ShouldReturnCorrectRate()
        {
            // Arrange
            var bcbResponse = new[] { new BcbApiResponse("01/01/2023", 10.5m) };
            var jsonResponse = JsonSerializer.Serialize(bcbResponse);
            SetupHttpClient(HttpStatusCode.OK, jsonResponse);

            // Act
            var result = await _taxaJurosProvider.GetTaxaJurosAtualAsync();

            // Assert
            result.Should().Be(10.5m);
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_WhenApiReturnsEmptyResponse_ShouldThrowException()
        {
            // Arrange
            var jsonResponse = "[]";
            SetupHttpClient(HttpStatusCode.OK, jsonResponse);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _taxaJurosProvider.GetTaxaJurosAtualAsync());
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_WhenApiReturnsErrorStatusCode_ShouldThrowHttpRequestException()
        {
            // Arrange
            SetupHttpClient(HttpStatusCode.InternalServerError, "Error");

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => _taxaJurosProvider.GetTaxaJurosAtualAsync());
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_WhenApiReturnsMalformedJson_ShouldThrowJsonException()
        {
            // Arrange
            SetupHttpClient(HttpStatusCode.OK, "{ \"data\": \"not-a-valid-json... ");

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => _taxaJurosProvider.GetTaxaJurosAtualAsync());
        }
    }
}