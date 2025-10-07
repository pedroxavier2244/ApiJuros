using FluentValidation;
using ApiJuros.Application.DTOs;

namespace ApiJuros.Application.Validators
{
    public class InvestmentInputValidator : AbstractValidator<InvestmentInput>
    {
   
        public InvestmentInputValidator() 
        {
            RuleFor(x => x.InitialValue)
                .GreaterThan(0).WithMessage("O valor inicial do investimento deve ser positivo."); 

            RuleFor(x => x.MonthlyInterestRate)
                .GreaterThan(0).WithMessage("A taxa de rendimento mensal deve ser positiva."); 

            RuleFor(x => x.TimeInMonths)
                .GreaterThan(0).WithMessage("O tempo em meses deve ser positivo."); 
        }
    }
}