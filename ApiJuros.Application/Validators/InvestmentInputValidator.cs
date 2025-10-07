using ApiJuros.Application.DTOs;
using FluentValidation;

namespace ApiJuros.Application.Validators
{
    public class InvestmentInputValidator : AbstractValidator<InvestmentInput>
    {
        public InvestmentInputValidator()
        {
            RuleFor(x => x.InitialValue)
                .GreaterThan(0)
                .WithMessage("O valor inicial do investimento deve ser positivo.");

            RuleFor(x => x.MonthlyInterestRate)
                .NotNull()
                .WithMessage("A taxa de juros mensal é obrigatória.");

            RuleFor(x => x.TimeInMonths)
                .GreaterThanOrEqualTo(0)
                .WithMessage("O tempo em meses não pode ser negativo.");
        }
    }
}