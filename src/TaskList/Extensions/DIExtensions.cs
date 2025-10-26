using TaskList.Interfaces;
using TaskList.Repositories;
using TaskList.SeedWork;
using TaskList.Services;

namespace TaskList.Extensions;

public static class DIExtensions
{
    public static WebApplicationBuilder ConfigureDI(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddScoped<IAuthService, AuthService>()
            .AddScoped<II18nService, I18nService>()
            .AddScoped<IMailService, MailService>()
            .AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>()
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<IPersonalRefreshTokenRepository, PersonalRefreshTokenRepository>()
            .AddScoped<ITaskRepository, TaskRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddScoped<IUserRepository, UserRepository>();

        return builder;
    }
}
