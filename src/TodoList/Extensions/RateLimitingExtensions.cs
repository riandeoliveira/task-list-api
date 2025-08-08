using Microsoft.AspNetCore.RateLimiting;
using TodoList.Dtos;
using TodoList.Interfaces;

namespace TodoList.Extensions;

public static class RateLimitingExtensions
{
    public static WebApplicationBuilder ConfigureRateLimiting(this WebApplicationBuilder builder)
    {
        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddFixedWindowLimiter(
                "Fixed",
                opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(60);
                    opt.PermitLimit = 100;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = System
                        .Threading
                        .RateLimiting
                        .QueueProcessingOrder
                        .OldestFirst;
                }
            );

            options.OnRejected = async (context, cancellationToken) =>
            {
                var i18n = context.HttpContext.RequestServices.GetRequiredService<II18nService>();

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                var response = new ProblemDetailsDto(
                    i18n.T("TooManyRequests"),
                    StatusCodes.Status429TooManyRequests,
                    context.HttpContext.Request.Path
                );

                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };
        });

        return builder;
    }

    public static WebApplication UseRateLimiting(this WebApplication app)
    {
        app.UseRateLimiter();

        return app;
    }
}
