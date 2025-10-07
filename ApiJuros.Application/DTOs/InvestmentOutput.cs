namespace ApiJuros.Application.DTOs
{
    public record InvestmentOutput
    {
        public decimal InvestedAmount { get; init; }
        public decimal FinalAmount { get; init; }
        public decimal TotalInterest { get; init; }
        public int TimeInMonths { get; init; }
    }
}