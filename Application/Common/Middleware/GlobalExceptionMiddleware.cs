using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace Application.Common.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        int statusCode = (int)HttpStatusCode.InternalServerError;
        string errorMessage = "An unexpected error occurred. Please try again later.";

        switch (exception)
        {
            case MySqlException mysqlEx:
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorMessage = "A database error occurred.";
                break;

            case DbUpdateException dbUpdateEx:
                statusCode = (int)HttpStatusCode.Conflict;
                errorMessage = "A database update error occurred.";
                break;

            case ValidationException validationEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                errorMessage = validationEx.Message;
                break;

            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                errorMessage = "Unauthorized access.";
                break;

            case ArgumentException argEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                errorMessage = argEx.Message;
                break;

            case ApplicationException appEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                errorMessage = appEx.Message;
                break;

            case NotFoundException notFoundEx:
                statusCode = (int)HttpStatusCode.NotFound;
                errorMessage = notFoundEx.Message;
                break;

            case NotFoundMessageException notFoundEx:
                statusCode = (int)HttpStatusCode.NotFound;
                errorMessage = notFoundEx.Message;
                break;

            case BadRequestException badReqEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                errorMessage = badReqEx.Message;
                break;

            case ConflictException confEx:
                statusCode = (int)HttpStatusCode.Conflict;
                errorMessage = confEx.Message;
                break;

            case UnsupportedMediaTypeException unsuppEx:
                statusCode = (int)HttpStatusCode.UnsupportedMediaType;
                errorMessage = unsuppEx.Message;
                break;

            case InternalErrorException intEx:
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorMessage = intEx.Message;
                break;

            case PayloadTooLargeException payEx:
                statusCode = (int)HttpStatusCode.RequestEntityTooLarge;
                errorMessage = payEx.Message;
                break;
        }

        response.StatusCode = statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            errorCode = statusCode,
            errorMessage
        });

        return response.WriteAsync(payload);
    }

}

public static class HttpContextExtensions
{
    public static string EnvironmentName(this HttpRequest request)
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    }
}

