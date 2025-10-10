using Xunit;
using FluentAssertions;
using ApiJuros.Application.DTOs;
using ApiJuros.Application.Services;
using Moq.AutoMock;
using AutoMapper;
using Moq;
using System.Threading.Tasks;
using ApiJuros.Application.Interfaces;
using ApiJuros.Domain;
using AutoFixture;

namespace ApiJuros.Test.Services
{
    public class FinancialCalculatorServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly Fixture _fixture;
        private readonly FinancialCalculatorService _service;

        public FinancialCalculatorServiceTests()
        {
            _mocker = new AutoMocker();
            _fixture = new Fixture();
            _service = _mocker.CreateInstance<FinancialCalculatorService>();
        }

        private void SetupMapper(InvestmentInput input)
        {
            var baseOutput = _fixture.Build<InvestmentOutput>()
                .With(x => x.InvestedAmount, input.InitialValue)
                .With(x => x.TimeInMonths, input.TimeInMonths)
                .Create();

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<InvestmentOutput>(input))
                .Returns(baseOutput);
        }

        [Fact]
        public void CalculateCompoundInterest_WithValidInputs_ShouldCalculateCorrectly()
        {
            // Arrange
            var input = _fixture.Build<InvestmentInput>()
                .With(i => i.InitialValue, 100)
                .With(i => i.MonthlyInterestRate, 1)
                .With(i => i.TimeInMonths, 12)
                .Create();

            SetupMapper(input);

            // Act
            var result = _service.CalculateCompoundInterest(input);

            // Assert
            result.FinalAmount.Should().BeApproximately(112.68m, 0.01m);
        }

        [Fact]
        public void CalculateCompoundInterest_WhenInterestRateIsZero_ReturnsInitialValue()
        {
            // Arrange
            var input = _fixture.Build<InvestmentInput>()
                .With(i => i.InitialValue, 1000)
                .With(i => i.MonthlyInterestRate, 0)
                .With(i => i.TimeInMonths, 24)
                .Create();

            SetupMapper(input);

            // Act
            var result = _service.CalculateCompoundInterest(input);

            // Assert
            result.FinalAmount.Should().Be(1000.00m);
            result.TotalInterest.Should().Be(0);
        }

        [Fact]
        public void CalculateCompoundInterest_WhenMonthsIsZero_ReturnsInitialValue()
        {
            // Arrange
            var input = _fixture.Build<InvestmentInput>()
                .With(i => i.InitialValue, 250.75m)
                .With(i => i.MonthlyInterestRate, 0.5m)
                .With(i => i.TimeInMonths, 0)
                .Create();

            SetupMapper(input);

            // Act
            var result = _service.CalculateCompoundInterest(input);

            // Assert
            result.FinalAmount.Should().Be(250.75m);
            result.TotalInterest.Should().Be(0);
        }

        [Fact]
        public async Task CalculateCompoundInterestWithAnnualRateAsync_WithAnnualRateOfOnePercent_ShouldCalculateCorrectly()
        {
            // Arrange
            decimal initialValue = 100;
            int months = 12;
            decimal annualInterestRate = 1; // 1% a.a
            decimal expectedAmount = 101.00m;

            _mocker.GetMock<IMapper>().Setup(m => m.Map<InvestmentOutput>(It.IsAny<InvestmentInput>()))
                .Returns((InvestmentInput i) => new InvestmentOutput { InvestedAmount = i.InitialValue, TimeInMonths = i.TimeInMonths });

            // Act
            var result = await _service.CalculateCompoundInterestWithAnnualRateAsync(initialValue, months, annualInterestRate);

            // Assert
            result.FinalAmount.Should().BeApproximately(expectedAmount, 0.01m);
            result.TotalInterest.Should().BeApproximately(expectedAmount - initialValue, 0.01m);
        }

        [Fact]
        public async Task CalculateCompoundInterestWithAnnualRateAsync_WithAnnualRateOfTwoPercent_ShouldCalculateCorrectly()
        {
            // Arrange
            decimal initialValue = 500;
            int months = 6;
            decimal annualInterestRate = 2; // 2% a.a.
            decimal expectedAmount = 504.97m;

            _mocker.GetMock<IMapper>().Setup(m => m.Map<InvestmentOutput>(It.IsAny<InvestmentInput>()))
                .Returns((InvestmentInput i) => new InvestmentOutput { InvestedAmount = i.InitialValue, TimeInMonths = i.TimeInMonths });

            // Act
            var result = await _service.CalculateCompoundInterestWithAnnualRateAsync(initialValue, months, annualInterestRate);

            // Assert
            result.FinalAmount.Should().BeApproximately(expectedAmount, 0.01m);
            result.TotalInterest.Should().BeApproximately(expectedAmount - initialValue, 0.01m);
        }
    }
}