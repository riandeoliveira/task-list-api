using Serilog;

namespace TaskList.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ConfigureLogger(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (_, logger) =>
                logger
                    .MinimumLevel.Debug()
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File("Logs/applog-.txt", rollingInterval: RollingInterval.Day)
        );

        return builder;
    }

    public static WebApplication UseLogger(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}
