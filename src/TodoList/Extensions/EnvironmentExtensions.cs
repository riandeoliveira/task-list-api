using DotNetEnv;

namespace TodoList.Extensions;

public static class EnvironmentExtensions
{
    public static WebApplicationBuilder ConfigureEnvironment(this WebApplicationBuilder builder)
    {
        var envPath = Path.Combine(AppContext.BaseDirectory, ".env");

        Env.Load(envPath);

        return builder;
    }
}
