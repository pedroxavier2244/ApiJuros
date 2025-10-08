using Xunit;
using FluentValidation.TestHelper;
using ApiJuros.Application.DTOs;
using ApiJuros.Application.Validators;

namespace ApiJuros.Test.Validators
{
    public class InvestmentInputValidatorTests
    {
        private readonly InvestmentInputValidator _validator;

        public InvestmentInputValidatorTests()
        {
            _validator = new InvestmentInputValidator();
        }

        [Fact]
        public void Should_Have_Error_When_InitialValue_Is_Zero()
        {
            var model = new InvestmentInput(0, 1, 12);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.InitialValue)
                  .WithErrorMessage("O valor inicial do investimento deve ser positivo.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_InitialValue_Is_Positive()
        {
            var model = new InvestmentInput(100, 1, 12);
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.InitialValue);
        }

        [Fact]
        public void Should_Have_Error_When_TimeInMonths_Is_Negative()
        {
            var model = new InvestmentInput(100, 1, -1);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.TimeInMonths)
                  .WithErrorMessage("O tempo em meses não pode ser negativo.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_TimeInMonths_Is_Zero()
        {
            var model = new InvestmentInput(100, 1, 0);
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.TimeInMonths);
        }

        [Fact]
        public void Should_Have_Error_When_MonthlyInterestRate_Is_Negative()
        {
            var model = new InvestmentInput(100, -1, 12);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.MonthlyInterestRate)
                .WithErrorMessage("A taxa de juros mensal não pode ser negativa.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_MonthlyInterestRate_Is_Zero()
        {
            var model = new InvestmentInput(100, 0, 12);
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveValidationErrorFor(x => x.MonthlyInterestRate);
        }
    }
}