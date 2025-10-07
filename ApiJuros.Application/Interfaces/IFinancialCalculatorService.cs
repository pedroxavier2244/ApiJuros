using ApiJuros.Application.DTOs;

namespace ApiJuros.Application.Interfaces
{
    public interface IFinancialCalculatorService
    {
        InvestmentOutput CalculateCompoundInterest(InvestmentInput input);
        Task<InvestmentOutput> CalculateCompoundInterestWithAnnualRateAsync(decimal initialValue, int timeInMonths, decimal annualInterestRate);

    }
}