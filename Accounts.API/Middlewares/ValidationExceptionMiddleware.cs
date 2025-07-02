using System.Net;
using FluentValidation;

namespace Accounts.API.Middlewares;

public class ValidationExceptionMiddleware(RequestDelegate next, ILogger<ValidationExceptionMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validation failed: {@Errors}", ex.Errors);

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .Select(e => new { e.PropertyName, e.ErrorMessage });

            var response = new
            {
                Message = "Validation Failed",
                Errors = errors
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}