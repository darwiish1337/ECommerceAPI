using System.Net;
using FluentValidation;
using SharedKernel.Exceptions;

namespace Accounts.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env)
{
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));
    private readonly IWebHostEnvironment _env = env ?? throw new ArgumentNullException(nameof(env));

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var errorDict = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).First() // أو يمكن استخدام `string.Join(", ", g.Select(e => e.ErrorMessage))`
                );

            await context.Response.WriteAsJsonAsync(new
            {
                errors = errorDict
            });
        }
        catch (ConflictException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            await context.Response.WriteAsJsonAsync(new
            {
                errors = new { Conflict = ex.Message }
            });
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                errors = new { Domain = ex.Message }
            });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = _env.IsDevelopment()
                ? new
                {
                    errors = new { Server = ex.Message }
                }
                : new
                {
                    errors = new { Server = "Something went wrong. Please try again later." }
                };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}