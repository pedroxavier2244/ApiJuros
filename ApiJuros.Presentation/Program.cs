using FluentValidation;
using FluentValidation.AspNetCore;
using ApiJuros.Application.Interfaces;
using ApiJuros.Application.Services;
using ApiJuros.Presentation.Middleware;

var builder = WebApplication.CreateBuilder(args); 


builder.Services.AddControllers(); 

builder.Services.AddFluentValidationAutoValidation(); 
builder.Services.AddValidatorsFromAssembly(typeof(IFinancialCalculatorService).Assembly); 
builder.Services.AddScoped<IFinancialCalculatorService, FinancialCalculatorService>(); 

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(); 

var app = builder.Build(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}


app.UseMiddleware<GlobalExceptionHandlerMiddleware>(); 
 
app.UseAuthorization(); 
app.MapControllers(); 
app.Run(); 