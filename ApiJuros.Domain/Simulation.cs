namespace ApiJuros.Domain
{
    public class Simulation
    {
        public Guid Id { get; set; }
        public decimal InitialValue { get; set; }
        public int TimeInMonths { get; set; }
        public decimal AnnualInterestRate { get; set; }
        public decimal FinalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}