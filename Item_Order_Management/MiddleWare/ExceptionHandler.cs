using System.Net;
using System.Text.Json;
using BusinessLogicLayer.Helper;

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
            await HandleExceptions(context, ex);
        }
    }

    private async Task HandleExceptions(HttpContext context, Exception exception)
    {
        HttpStatusCode code;
        string message = NotificationMessage.SomethingWentWrong;

        code = context.Response.StatusCode switch
        {
            400 => HttpStatusCode.BadRequest,
            401 => HttpStatusCode.Unauthorized,
            404 => HttpStatusCode.NotFound,
            500 => HttpStatusCode.InternalServerError,
            _ => HttpStatusCode.InternalServerError
        };
        

        _logger.LogError(exception, NotificationMessage.UnhandledExceptionMessage);

        bool isAjax = context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        if (isAjax)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200; // Always OK (to avoid redirect issues)

            context.Response.Headers.Add("X-Error", "true");

            var jsonResponse = new
            {
                success = false,
                statusCode = (int)code,
                error = message
            };

            string jsonMessage = JsonSerializer.Serialize(jsonResponse);
            await context.Response.WriteAsync(jsonMessage);
        }
        else
        {
            if (!context.Response.HasStarted)
            {
                var redirectUrl = $"/Error/InternalServerError";
                context.Response.StatusCode = (int)HttpStatusCode.Redirect;
                context.Response.Headers["Location"] = redirectUrl;
                await context.Response.CompleteAsync();
            }
            else
            {
                _logger.LogWarning(NotificationMessage.ResponseStartedLogWarning);
                context.Response.StatusCode = (int)code;
                await context.Response.WriteAsync(message);
            }
        }
    }
}