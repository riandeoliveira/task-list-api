using Serilog;

namespace TodoList.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (context, config) => config.ReadFrom.Configuration(context.Configuration)
        );

        return builder;
    }

    public static WebApplication UseLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        return app;
    }
}
