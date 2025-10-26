using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskList.Constants;
using TaskList.Dtos;
using TaskList.Interfaces;

namespace TaskList.Extensions;

public static class JwtAuthExtensions
{
    public static WebApplicationBuilder ConfigureAuth(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddAuthorization()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(EnvironmentVariables.JwtSecret)
                    ),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidAudience = EnvironmentVariables.JwtAudience,
                    ValidIssuer = EnvironmentVariables.JwtIssuer,
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = async context =>
                    {
                        var i18n =
                            context.HttpContext.RequestServices.GetRequiredService<II18nService>();

                        var errorMessage = i18n.T("UnauthorizedOperation");

                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/problem+json";

                        var response = new ProblemDetailsDto(
                            errorMessage,
                            context.Response.StatusCode,
                            context.Request.Path
                        );

                        await context.Response.WriteAsJsonAsync(response);
                    },
                };
            });

        return builder;
    }

    public static WebApplication UseAuth(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
