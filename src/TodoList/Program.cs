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
    .ConfigureJwtAuth()
    .ConfigureRateLimiting()
    .Build()
    .UseLogging()
    .UseApiDoc()
    .UseI18n()
    .UseHttp()
    .UseJwtAuth()
    .UseRateLimiting()
    .RunAsync();
