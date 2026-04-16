using B2B.Common.Exceptions;
using B2B.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace B2B.Common.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        _next = next;
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
            Log.Error(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        // Retrieve our custom Correlation ID instead of the default local TraceIdentifier
        context.Response.Headers.TryGetValue("X-Correlation-ID", out var correlationId);

        var response = new ErrorResponse
        {
            TraceId = correlationId.FirstOrDefault() ?? context.TraceIdentifier
        };

        if (exception is AppException appEx)
        {
            context.Response.StatusCode = (int)appEx.StatusCode;
            response.Message = appEx.Message;
        }
        else
        {
            // For security, we don't leak non-custom exceptions to the client
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Message = "An internal server error occurred. Please contact support.";
        }

        if (_env.IsDevelopment())
        {
            response.DebugInfo = exception.ToString();
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        });

        await context.Response.WriteAsync(json);
    }
}
