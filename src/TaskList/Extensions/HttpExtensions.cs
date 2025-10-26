using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TaskList.Dtos;
using TaskList.Interfaces;
using TaskList.Middlewares;
using TaskList.Utils;

namespace TaskList.Extensions;

public static class HttpExtensions
{
    public static WebApplicationBuilder ConfigureHttp(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<Program>();

        builder
            .Services.AddHttpContextAccessor()
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy =
                    JsonNamingPolicy.SnakeCaseLower;
            });

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var i18n = context.HttpContext.RequestServices.GetRequiredService<II18nService>();

                var errors = context
                    .ModelState.Where(kvp => kvp.Value != null && kvp.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp =>
                            string.IsNullOrWhiteSpace(kvp.Key)
                                ? "general"
                                : TextUtil.ToSnakeCase(kvp.Key),
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var response = new ProblemDetailsDto(
                    i18n.T("UnexpectedRequestError"),
                    StatusCodes.Status400BadRequest,
                    context.HttpContext.Request.Path,
                    errors
                );

                return new BadRequestObjectResult(response);
            };
        });

        return builder;
    }

    public static WebApplication UseHttp(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        app.UseHttpsRedirection();
        app.MapControllers().RequireRateLimiting("Fixed");

        return app;
    }
}
