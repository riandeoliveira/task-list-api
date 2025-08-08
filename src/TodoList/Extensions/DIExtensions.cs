using TodoList.Handlers;
using TodoList.Interfaces;
using TodoList.Repositories;
using TodoList.SeedWork;
using TodoList.Services;

namespace TodoList.Extensions;

public static class DIExtensions
{
    public static WebApplicationBuilder ConfigureDI(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddScoped<IAuthService, AuthService>()
            .AddScoped<II18nService, I18nService>()
            .AddScoped<IMailService, MailService>()
            .AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>()
            .AddScoped<IPersonalRefreshTokenRepository, PersonalRefreshTokenRepository>()
            .AddScoped<ITodoHandler, TodoHandler>()
            .AddScoped<ITodoRepository, TodoRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IUserHandler, UserHandler>()
            .AddScoped<IUserRepository, UserRepository>();

        return builder;
    }
}
