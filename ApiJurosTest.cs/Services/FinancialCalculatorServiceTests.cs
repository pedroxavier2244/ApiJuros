using ApiJuros.Application.DTOs;
using ApiJuros.Application.Services;
using AutoFixture;
using FluentAssertions;
using Moq.AutoMock;
using Xunit;

namespace ApiJuros.Test.Services
{
    public class FinancialCalculatorServiceTests
    {
        private readonly Fixture _fixture;
        private readonly AutoMocker _mocker;

        public FinancialCalculatorServiceTests()
        {
            _fixture = new Fixture();
            _mocker = new AutoMocker();
        }

        [Fact]
        public void CalculateCompoundInterest_ShouldCalculateCorrectly()
        {
            // Arrange
            var input = _fixture.Build<InvestmentInput>()
                .With(i => i.InitialValue, 100)
                .With(i => i.TimeInMonths, 12)
                .With(i => i.MonthlyInterestRate, 1)
                .Create();

            var service = _mocker.CreateInstance<FinancialCalculatorService>();

            // Act
            var result = service.CalculateCompoundInterest(input);

            // Assert
            result.Should().NotBeNull();
            result.FinalAmount.Should().Be(112.68m); 
            result.TotalInterest.Should().Be(12.68m);
            result.InvestedAmount.Should().Be(100);
            result.TimeInMonths.Should().Be(12);
        }
    }
}
