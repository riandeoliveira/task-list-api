using TodoList.Extensions;

await WebApplication
    .CreateBuilder()
    .ConfigureEnvironment()
    .ConfigureLogging()
    .ConfigureDbContext()
    .ConfigureDI()
    .ConfigureApiDoc()
    .ConfigureI18n()
    .ConfigureHttp()
    .ConfigureCors()
    .ConfigureAuth()
    .ConfigureRateLimiting()
    .Build()
    .UseLogging()
    .UseApiDoc()
    .UseI18n()
    .UseHttp()
    .UseCors()
    .UseAuth()
    .UseRateLimiting()
    .RunAsync();
