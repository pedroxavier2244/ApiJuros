using ApiJuros.Application.Interfaces;
using ApiJuros.Application.Services;
using ApiJuros.Domain;
using ApiJuros.Infrastructure.Auth;
using ApiJuros.Infrastructure.Persistence;
using ApiJuros.Infrastructure.Repositories;
using ApiJuros.Presentation.Middleware;
using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Net;
using System.Text;
using ApiJuros.Application.Settings;

var builder = WebApplication.CreateBuilder(args);



builder.Host.UseSerilog((context, configuration) =>

    configuration.ReadFrom.Configuration(context.Configuration));



builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>

{

    options.Password.RequireDigit = true;

    options.Password.RequireLowercase = true;

    options.Password.RequireUppercase = true;

    options.Password.RequireNonAlphanumeric = false;

    options.Password.RequiredLength = 8;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

    options.Lockout.MaxFailedAccessAttempts = 5;

    options.Lockout.AllowedForNewUsers = true;

    options.User.RequireUniqueEmail = true;

})

.AddEntityFrameworkStores<ApiJurosDbContext>()

.AddDefaultTokenProviders();



var jwtSettings = new JwtSettings();

builder.Configuration.Bind("JwtSettings", jwtSettings);



if (string.IsNullOrEmpty(jwtSettings.Secret) || string.IsNullOrEmpty(jwtSettings.Issuer) || string.IsNullOrEmpty(jwtSettings.Audience))

{

    throw new InvalidOperationException("As configurações do JWT (Secret, Issuer, Audience) não estão completas em appsettings.json.");

}



builder.Services.AddSingleton(jwtSettings);



builder.Services.AddAuthentication(options =>

{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

})

.AddJwtBearer(options =>

{

    options.SaveToken = true;

    options.RequireHttpsMetadata = builder.Environment.IsProduction();

    options.TokenValidationParameters = new TokenValidationParameters

    {

        ValidateIssuer = true,

        ValidateAudience = true,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,

        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

        ClockSkew = TimeSpan.Zero

    };

});



builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddValidatorsFromAssembly(typeof(IFinancialCalculatorService).Assembly);



builder.Services.AddScoped<IFinancialCalculatorService, FinancialCalculatorService>();

builder.Services.AddScoped<ISimulationRepository, SimulationRepository>();

builder.Services.AddScoped<ITokenService, TokenService>();



builder.Services.AddAutoMapper(typeof(IFinancialCalculatorService).Assembly);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ApiJuros_"; 
});

builder.Services.AddDbContext<ApiJurosDbContext>(options =>

    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));



builder.Services.AddHttpClient<ITaxaJurosProvider, TaxaJurosProvider>()

    .AddPolicyHandler((serviceProvider, request) => HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound).WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), onRetry: (outcome, timespan, retryAttempt, context) => { var logger = serviceProvider.GetRequiredService<ILogger<TaxaJurosProvider>>(); logger.LogWarning("Falha ao chamar a API do BCB (Status: {StatusCode}). Tentando novamente em {timespan}. Tentativa {retryAttempt}", outcome.Result?.StatusCode, timespan, retryAttempt); }))

    .AddPolicyHandler((serviceProvider, request) => HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound).CircuitBreakerAsync(5, TimeSpan.FromSeconds(30), onBreak: (outcome, timespan, context) => { var logger = serviceProvider.GetRequiredService<ILogger<TaxaJurosProvider>>(); logger.LogError("Circuito aberto por 30 segundos devido a falhas consecutivas."); }, onReset: (context) => { var logger = serviceProvider.GetRequiredService<ILogger<TaxaJurosProvider>>(); logger.LogInformation("Circuito fechado. As chamadas voltarão ao normal."); }));



var healthChecksBuilder = builder.Services.AddHealthChecks();

healthChecksBuilder.AddUrlGroup(new Uri("https://api.bcb.gov.br/dados/serie/bcdata.sgs.432/dados/ultimos/1?formato=json"), name: "api_bcb_selic", tags: new string[] { "api", "externa" });

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(connectionString)) { healthChecksBuilder.AddNpgSql(connectionString, name: "database", tags: new string[] { "db", "postgresql" }); }

builder.Services.AddHealthChecksUI().AddInMemoryStorage();



builder.Services.AddApiVersioning(options => { options.ReportApiVersions = true; options.AssumeDefaultVersionWhenUnspecified = true; options.DefaultApiVersion = new ApiVersion(1, 0); }).AddApiExplorer(options => { options.GroupNameFormat = "'v'VVV"; options.SubstituteApiVersionInUrl = true; });



builder.Services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Juros v1", Version = "v1" }); c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Name = "Authorization", Type = SecuritySchemeType.ApiKey, Scheme = "Bearer", BearerFormat = "JWT", In = ParameterLocation.Header, Description = "Insira o token JWT com o prefixo Bearer. Exemplo: \"Bearer {seu_token}\"" }); c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } }); });



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



app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();



app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

app.MapHealthChecksUI(options => { options.UIPath = "/health-ui"; });



app.MapControllers();



app.Run();
