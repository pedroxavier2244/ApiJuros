using Microsoft.AspNetCore.Mvc;
using ApiJuros.Application.DTOs;
using ApiJuros.Application.Interfaces;

namespace ApiJuros.Presentation.Controllers;


[ApiController] 
[Route("api/[controller]")] 

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