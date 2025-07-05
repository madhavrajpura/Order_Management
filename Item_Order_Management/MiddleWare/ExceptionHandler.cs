using System.Net;
using System.Text.Json;
using BusinessLogicLayer.Helper;
using Microsoft.EntityFrameworkCore;

namespace Item_Order_Management.MiddleWare;

public class ExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionsAsync(context, ex);   
        }
    }

    private async Task HandleExceptionsAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode;
        string message;

        if (exception is CustomException customException)
        {
            // Use CustomException message
            statusCode = HttpStatusCode.BadRequest;
            message = customException.Message;
        }
        else
        {
            // For other unhandled exceptions
            statusCode = context.Response.StatusCode switch
            {
                400 => HttpStatusCode.BadRequest,
                401 => HttpStatusCode.Unauthorized,
                404 => HttpStatusCode.NotFound,
                500 => HttpStatusCode.InternalServerError,
                _ => HttpStatusCode.InternalServerError
            };
            message = NotificationMessage.SomethingWentWrong;
            _logger.LogError(exception, NotificationMessage.UnhandledExceptionMessage);
        }

        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        if (isAjax)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode; // Kept 200 for AJAX to avoid redirect issues

            context.Response.Headers.Add("X-Error", "true");

            var jsonResponse = new
            {
                success = false,
                statusCode = (int)statusCode,
                error = message
            };

            string jsonMessage = JsonSerializer.Serialize(jsonResponse);
            await context.Response.WriteAsync(jsonMessage);
        }
        else
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Redirect;
                context.Response.Headers["Location"] = "/Error/InternalServerError";
                await context.Response.CompleteAsync();
            }
            else
            {
                // Log warning only for non-CustomException cases
                if (!(exception is CustomException))
                    _logger.LogWarning(NotificationMessage.ResponseStartedLogWarning);
                context.Response.StatusCode = (int)statusCode;
                await context.Response.WriteAsync(message);
            }
        }
    }
}