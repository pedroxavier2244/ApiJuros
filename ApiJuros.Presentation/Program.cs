using ApiJuros.Application.Interfaces;
using ApiJuros.Application.Services;
using ApiJuros.Presentation.Middleware;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(IFinancialCalculatorService).Assembly);
builder.Services.AddScoped<IFinancialCalculatorService, FinancialCalculatorService>();
builder.Services.AddAutoMapper(typeof(IFinancialCalculatorService).Assembly);



builder.Services.AddHttpClient();
builder.Services.AddScoped<ITaxaJurosProvider, TaxaJurosProvider>();

var healthChecksBuilder = builder.Services.AddHealthChecks();

healthChecksBuilder.AddUrlGroup(
    new Uri("https://api.bcb.gov.br/dados/serie/bcdata.sgs.11/dados/ultimos/1?formato=json"),
    name: "api_bcb_selic",
    tags: new string[] { "api", "externa" });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString))
{
    healthChecksBuilder.AddNpgSql(
        connectionString,
        name: "database",
        tags: new string[] { "db", "postgresql" });
}


builder.Services.AddHealthChecksUI()
    .AddInMemoryStorage();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Juros v1", Version = "v1" });
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.MapControllers();
app.Run();