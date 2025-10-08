using Xunit;
using FluentAssertions;
using ApiJuros.Application.DTOs;
using ApiJuros.Application.Services;
using Moq.AutoMock;
using AutoMapper;
using Moq;
using System.Threading.Tasks;

namespace ApiJuros.Test.Services
{
    public class FinancialCalculatorServiceTests
    {
        private readonly AutoMocker _mocker;
        private readonly FinancialCalculatorService _service;

        public FinancialCalculatorServiceTests()
        {
            _mocker = new AutoMocker();
            // O AutoMocker criará uma instância do serviço e injetará mocks de IMapper e ILogger.
            _service = _mocker.CreateInstance<FinancialCalculatorService>();
        }

        private void SetupMapper(InvestmentInput input)
        {
            // Configura o mock do IMapper para retornar um objeto base quando for chamado.
            // Isso isola o teste da lógica real de mapeamento.
            var baseOutput = new InvestmentOutput
            {
                InvestedAmount = input.InitialValue,
                TimeInMonths = input.TimeInMonths
            };

            _mocker.GetMock<IMapper>()
                .Setup(m => m.Map<InvestmentOutput>(input))
                .Returns(baseOutput);
        }

        [Fact]
        public void CalculateCompoundInterest_WithValidInputs_ShouldCalculateCorrectly()
        {
            // Arrange
            var input = new InvestmentInput(100, 1, 12);
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
            var input = new InvestmentInput(1000, 0, 24);
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
            var input = new InvestmentInput(250.75m, 0.5m, 0);
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
            decimal expectedAmount = 101.00m; // Valor correto é 101.00

            // O método interno de cálculo também usa o Mapper, então precisamos configurá-lo.
            // O AutoMocker é inteligente o suficiente para usar o mesmo mock.
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
            decimal expectedAmount = 504.97m; // Valor correto é 504.97

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