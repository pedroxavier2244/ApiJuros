namespace ApiJuros.Application.DTOs
{
    public record InvestmentInput(
        decimal InitialValue,         
        decimal MonthlyInterestRate,  
        int TimeInMonths              
    );
}