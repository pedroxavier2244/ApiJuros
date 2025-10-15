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
using System;

namespace ApiJuros.Test.Services
{
    public class TaxaJurosProviderTests
    {
        private readonly Mock<ILogger<TaxaJurosProvider>> _loggerMock;

        public TaxaJurosProviderTests()
        {
            _loggerMock = new Mock<ILogger<TaxaJurosProvider>>();
        }

        private HttpClient CreateMockHttpClient(HttpStatusCode statusCode, string jsonResponse)
        {
            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(jsonResponse)
                });

            return new HttpClient(httpMessageHandlerMock.Object);
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_WhenApiReturnsValidData_ShouldReturnCorrectRate()
        {
            // Arrange
            var bcbResponse = new[] { new BcbApiResponse("08/10/2025", 15.0m) };
            var jsonResponse = JsonSerializer.Serialize(bcbResponse);

            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);

            var taxaJurosProvider = new TaxaJurosProvider(httpClient, _loggerMock.Object);

            // Act
            var result = await taxaJurosProvider.GetTaxaJurosAtualAsync();

            // Assert
            result.Should().Be(15.0m);
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_WhenApiReturnsEmptyResponse_ShouldThrowException()
        {
            // Arrange
            var jsonResponse = "[]";
            var httpClient = CreateMockHttpClient(HttpStatusCode.OK, jsonResponse);
            var taxaJurosProvider = new TaxaJurosProvider(httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => taxaJurosProvider.GetTaxaJurosAtualAsync());
        }

        [Fact]
        public async Task GetTaxaJurosAtualAsync_WhenApiReturnsErrorStatusCode_ShouldThrowHttpRequestException()
        {
            // Arrange
            var httpClient = CreateMockHttpClient(HttpStatusCode.InternalServerError, "Error");
            var taxaJurosProvider = new TaxaJurosProvider(httpClient, _loggerMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => taxaJurosProvider.GetTaxaJurosAtualAsync());
        }
    }
}