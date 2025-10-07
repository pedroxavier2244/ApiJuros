using Microsoft.AspNetCore.Mvc;
using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;
using Asp.Versioning; 

namespace ApiJuros.Presentation.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class FinancialCalculatorController : ControllerBase
    {
        private readonly IFinancialCalculatorService _calculatorService;

        public FinancialCalculatorController(IFinancialCalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }  

        [HttpPost("calculate-compound-interest")]
        public IActionResult CalculateCompoundInterest([FromBody] InvestmentInput investment)
        {
            var result = _calculatorService.CalculateCompoundInterest(investment);
            return Ok(result);
        }
    }
}