using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ApiJuros.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FinancialCalculatorController : ControllerBase
    {
        private readonly IFinancialCalculatorService _calculatorService;
        private readonly ITaxaJurosProvider _taxaJurosProvider;

        public FinancialCalculatorController(
            IFinancialCalculatorService calculatorService,
            ITaxaJurosProvider taxaJurosProvider)
        {
            _calculatorService = calculatorService;
            _taxaJurosProvider = taxaJurosProvider;
        }

        [HttpPost("calculate-compound-interest")]
        public IActionResult CalculateCompoundInterest([FromBody] InvestmentInput investment)
        {
            var result = _calculatorService.CalculateCompoundInterest(investment);
            return Ok(result);
        }

        [HttpGet("current-interest-rate")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCurrentInterestRate()
        {
            var taxaJuros = await _taxaJurosProvider.GetTaxaJurosAtualAsync();
            return Ok(new { currentInterestRate = taxaJuros });
        }

        [HttpPost("calculate-with-selic-rate")]
        [ProducesResponseType(typeof(InvestmentOutput), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CalculateWithSelicRate([FromBody] InvestmentInputWithoutRate input)
        {
            var taxaSelicAnual = await _taxaJurosProvider.GetTaxaJurosAtualAsync();

            var result = await _calculatorService.CalculateCompoundInterestWithAnnualRateAsync(
                input.InitialValue,
                input.TimeInMonths,
                taxaSelicAnual
            );

            return Ok(result);
        }
    }
}