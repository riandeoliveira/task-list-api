using TaskList.Extensions;

await WebApplication
    .CreateBuilder()
    .ConfigureEnvironment()
    .ConfigureLogger()
    .ConfigureDbContext()
    .ConfigureDI()
    .ConfigureApiDoc()
    .ConfigureI18n()
    .ConfigureHttp()
    .ConfigureCors()
    .ConfigureAuth()
    .ConfigureRateLimiting()
    .Build()
    .UseLogger()
    .UseApiDoc()
    .UseI18n()
    .UseHttp()
    .UseCors()
    .UseAuth()
    .UseRateLimiting()
    .RunAsync();
