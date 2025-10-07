using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;

namespace ApiJuros.Application.Services
{
    public class FinancialCalculatorService : IFinancialCalculatorService
    {

        public InvestmentOutput CalculateCompoundInterest(InvestmentInput input) 
        {
            var decimalRate = input.MonthlyInterestRate / 100; 
            var finalAmount = input.InitialValue * (decimal)Math.Pow(1 + (double)decimalRate, input.TimeInMonths); 
            var roundedFinalAmount = Math.Round(finalAmount, 2); 
            var totalInterest = roundedFinalAmount - input.InitialValue; 

            return new InvestmentOutput 
            {
                InvestedAmount = input.InitialValue, 
                FinalAmount = roundedFinalAmount,
                TotalInterest = totalInterest, 
                TimeInMonths = input.TimeInMonths 
            };
        }
    }
}