using Xunit;
using FluentAssertions;
using ApiJuros.Application.DTOs;
using ApiJuros.Application.Services;
using Moq.AutoMock;
using AutoMapper;
using Moq;
using ApiJuros.Application.Mappings;

namespace ApiJuros.Test.Services
{
    public class FinancialCalculatorServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly FinancialCalculatorService _service;

        public FinancialCalculatorServiceTests()
        {
            _mocker = new AutoMocker();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = mapperConfig.CreateMapper();
            _mocker.Use<IMapper>(mapper);

            _service = _mocker.CreateInstance<FinancialCalculatorService>();
        }

        [Theory]
        [InlineData(100, 1, 12, 112.68)]
        [InlineData(500, 2.5, 6, 579.85)]
        [InlineData(1000, 0, 24, 1000.00)]
        [InlineData(250.75, 0.5, 0, 250.75)]
        [InlineData(1, 10, 1, 1.10)]
        public void CalculateCompoundInterest_WithVariousInputs_ShouldCalculateCorrectly(
            decimal initialValue, decimal interestRate, int months, decimal expectedAmount)
        {
            // Arrange
            var input = new InvestmentInput(initialValue, interestRate, months);
            var baseOutput = new InvestmentOutput
            {
                InvestedAmount = initialValue,
                TimeInMonths = months
            };

            // Act
            var result = _service.CalculateCompoundInterest(input);

            // Assert
            result.Should().NotBeNull();
            result.FinalAmount.Should().BeApproximately(expectedAmount, 0.01m, "o clculo do valor final deve ser preciso");
            result.InvestedAmount.Should().Be(initialValue);
            result.TimeInMonths.Should().Be(months);
            result.TotalInterest.Should().BeApproximately(expectedAmount - initialValue, 0.01m, "o clculo do juros total deve ser preciso");
        }

        [Theory]
        [InlineData(100, 12, 1, 100.95)]
        [InlineData(500, 6, 2, 504.94)]
        public async Task CalculateCompoundInterestWithAnnualRateAsync_WithVariousInputs_ShouldCalculateCorrectly(
            decimal initialValue, int months, decimal annualInterestRate, decimal expectedAmount)
        {
            // Arrange

            // Act
            var result = await _service.CalculateCompoundInterestWithAnnualRateAsync(initialValue, months, annualInterestRate);

            // Assert
            result.Should().NotBeNull();
            result.FinalAmount.Should().BeApproximately(expectedAmount, 0.05m, "o calculo do valor final deve ser preciso");
            result.InvestedAmount.Should().Be(initialValue);
            result.TimeInMonths.Should().Be(months);
            result.TotalInterest.Should().BeApproximately(expectedAmount - initialValue, 0.05m, "o calculo do juros total deve ser preciso");
        }
    }
}