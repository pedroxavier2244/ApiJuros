using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;

namespace ApiJuros.Application.Services
{
    public class FinancialCalculatorService : IFinancialCalculatorService
    {
        private const int PercentageDivisor = 100;
        private const int DecimalPlacesToRound = 2;
        private readonly IMapper _mapper;
        private readonly ILogger<FinancialCalculatorService> _logger;

        public FinancialCalculatorService(IMapper mapper, ILogger<FinancialCalculatorService> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        public InvestmentOutput CalculateCompoundInterest(InvestmentInput input)
        {
            _logger.LogInformation("Iniciando cálculo de juros para o valor inicial {InitialValue}", input.InitialValue);

            var monthlyRate = input.MonthlyInterestRate / PercentageDivisor;

            if (input.TimeInMonths == 0)
            {
                _logger.LogWarning("O cálculo foi solicitado para 0 meses. O juros total será zero.");
            }

            decimal finalAmount = input.InitialValue;
            for (int i = 0; i < input.TimeInMonths; i++)
            {
                finalAmount *= (1 + monthlyRate);
            }

            var roundedFinalAmount = Math.Round(finalAmount, DecimalPlacesToRound);
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

        public Task<InvestmentOutput> CalculateCompoundInterestWithAnnualRateAsync(decimal initialValue, int timeInMonths, decimal annualInterestRate)
        {
            var monthlyInterestRate = ConverterTaxaAnualParaMensalEquivalente(annualInterestRate);

            var input = new InvestmentInput(initialValue, monthlyInterestRate, timeInMonths);

            var result = CalculateCompoundInterest(input);

            return Task.FromResult(result);
        }

        private decimal ConverterTaxaAnualParaMensalEquivalente(decimal taxaAnualPercentual)
        {
            var taxaAnual = taxaAnualPercentual / 100.0m;
            var taxaMensal = (decimal)Math.Pow(1 + (double)taxaAnual, 1.0 / 12.0) - 1;
            return taxaMensal * 100;
        }
    }
}
