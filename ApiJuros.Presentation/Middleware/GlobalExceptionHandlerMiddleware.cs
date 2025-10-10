using System.Net;
using System.Text.Json;

namespace ApiJuros.Presentation.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            object response;

            if (_env.IsDevelopment())
            {
                response = new
                {
                    error = new
                    {
                        message = "Ocorreu um erro inesperado.",
                        details = exception.ToString()
                    }
                };
            }
            else
            {
                response = new
                {
                    error = new
                    {
                        message = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
                    }
                };
            }

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}