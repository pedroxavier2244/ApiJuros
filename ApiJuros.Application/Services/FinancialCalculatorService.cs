using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;
using ApiJuros.Domain;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ApiJuros.Application.Services
{
    public class FinancialCalculatorService : IFinancialCalculatorService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<FinancialCalculatorService> _logger;
        private readonly ISimulationRepository _simulationRepository;
        private const int PercentageDivisor = 100;
        private const int DecimalPlacesToRound = 2;

        public FinancialCalculatorService(IMapper mapper, ILogger<FinancialCalculatorService> logger, ISimulationRepository simulationRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _simulationRepository = simulationRepository;
        }

        public InvestmentOutput CalculateCompoundInterest(InvestmentInput input)
        {
            _logger.LogInformation("Iniciando cálculo de juros para o valor inicial {InitialValue}", input.InitialValue);

            var monthlyRate = input.MonthlyInterestRate / PercentageDivisor; 

            if (input.TimeInMonths == 0)
            {
                _logger.LogWarning("O cálculo foi solicitado para 0 meses. O juros total será zero.");
            }

            decimal finalAmount = input.InitialValue * (decimal)Math.Pow(1 + (double)monthlyRate, input.TimeInMonths);
            decimal roundedFinalAmount = Math.Round(finalAmount, DecimalPlacesToRound);

            var totalInterest = roundedFinalAmount - input.InitialValue;

            var output = _mapper.Map<InvestmentOutput>(input);
            var finalResult = output with
            {
                FinalAmount = roundedFinalAmount,
                TotalInterest = totalInterest
            };

            _logger.LogInformation("Cálculo finalizado. Valor final: {FinalAmount}", finalResult.FinalAmount);
            return finalResult;
        }

        public async Task<InvestmentOutput> CalculateCompoundInterestWithAnnualRateAsync(decimal initialValue, int timeInMonths, decimal annualInterestRate)
        {
            var taxaAnual = annualInterestRate / 100.0m;
            var taxaMensalDecimal = (decimal)Math.Pow(1 + (double)taxaAnual, 1.0 / 12.0) - 1;

            var monthlyInterestRate = taxaMensalDecimal * 100;

            var input = new InvestmentInput(initialValue, monthlyInterestRate, timeInMonths);

            var result = CalculateCompoundInterest(input);

            _logger.LogInformation("Salvando simulação no banco de dados...");
            var simulation = new Simulation
            {
                Id = Guid.NewGuid(),
                InitialValue = initialValue,
                TimeInMonths = timeInMonths,
                AnnualInterestRate = annualInterestRate,
                FinalAmount = result.FinalAmount,
                CreatedAt = DateTime.UtcNow
            };
            await _simulationRepository.AddAsync(simulation);
            _logger.LogInformation("Simulação salva com sucesso.");

            return result;
        }
        private decimal ConverterTaxaAnualParaMensal(decimal taxaAnualPercentual)
        {
            var taxaAnual = taxaAnualPercentual / 100.0m;
            var taxaMensal = (decimal)Math.Pow(1 + (double)taxaAnual, 1.0 / 12.0) - 1;
            return taxaMensal * 100; 
        }
    }
}