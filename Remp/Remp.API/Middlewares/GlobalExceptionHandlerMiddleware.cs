using System;
using Remp.Remp.Common.Responses;
using System.Text.Json;
using FluentValidation;
namespace Remp.Remp.API.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation exception occurred.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            APIResponses<object> response = new APIResponses<object>
            {
                Success = false,
                Message = "Validation failed.",
                Data = null,
                Errors = ex.Errors.Select(x => x.ErrorMessage).ToList()
            };

            string jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogError(ex, "Resource not found.");

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            APIResponses<object> response = new APIResponses<object>
            {
                Success = false,
                Message = ex.Message,
                Data = null,
                Errors = new List<string> { ex.Message }
            };

            string jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            APIResponses<object> response = new APIResponses<object>
            {
                Success = false,
                Message = "An unexpected error occurred.",
                Data = null,
                Errors = new List<string> { ex.Message }
            };

            string jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
