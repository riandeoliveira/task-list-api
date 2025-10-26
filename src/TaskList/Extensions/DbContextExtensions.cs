using Microsoft.EntityFrameworkCore;
using TaskList.Constants;
using TaskList.Contexts;

namespace TaskList.Extensions;

public static class DbContextExtensions
{
    public static WebApplicationBuilder ConfigureDbContext(this WebApplicationBuilder builder)
    {
        var port = builder.Environment.IsDevelopment() ? EnvironmentVariables.DatabasePort : 5432;

        var connectionString =
            $@"
            Server={EnvironmentVariables.DatabaseHost};
            Port={port};
            Database={EnvironmentVariables.DatabaseName};
            Username={EnvironmentVariables.DatabaseUser};
            Password={EnvironmentVariables.DatabasePassword}
        ";

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return builder;
    }

    public static WebApplication UseDbContext(this WebApplication app)
    {
        if (app.Environment.IsProduction())
        {
            using var scope = app.Services.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            service.Database.Migrate();
        }

        return app;
    }
}
