using TaskList.Constants;

namespace TaskList.Extensions;

public static class CorsExtensions
{
    public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                policy =>
                {
                    policy
                        .WithOrigins(EnvironmentVariables.ClientUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            );
        });

        return builder;
    }

    public static WebApplication UseCors(this WebApplication app)
    {
        app.UseCors("CorsPolicy");

        return app;
    }
}
