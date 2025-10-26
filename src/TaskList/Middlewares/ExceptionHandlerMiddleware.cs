using TaskList.Dtos;
using TaskList.Exceptions;

namespace TaskList.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var instancePath = context.Request.Path;
        var exceptionMessage = exception.Message;

        var statusCode = exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            NotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError,
        };

        var response = new ProblemDetailsDto(exceptionMessage, statusCode, instancePath);

        context.Response.StatusCode = response.Status;

        await context.Response.WriteAsJsonAsync(response);
    }
}
