using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace TaskList.Extensions;

public static class I18nExtensions
{
    public static WebApplicationBuilder ConfigureI18n(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddLocalization(options => options.ResourcesPath = "Resources")
            .Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo> { new("en-US"), new("pt-BR") };

                options.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

        return builder;
    }

    public static WebApplication UseI18n(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

        app.UseRequestLocalization(options);

        return app;
    }
}
