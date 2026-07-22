
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RotaCerta.API.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IWebHostEnvironment env)
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
            _logger.LogError(ex, "Erro inesperado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var mensagem = _env.IsDevelopment()
            ? $"{ex.Message} {ex?.InnerException?.Message}"
            : "Erro interno do servidor. Tente novamente.";

        var result = new GlobalErroResponse(mensagem);

        await context.Response.WriteAsJsonAsync(result);
    }
}