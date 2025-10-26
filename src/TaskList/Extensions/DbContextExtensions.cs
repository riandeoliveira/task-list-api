using Microsoft.EntityFrameworkCore;
using TaskList.Constants;
using TaskList.Contexts;

namespace TaskList.Extensions;

public static class DbContextExtensions
{
    public static WebApplicationBuilder ConfigureDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(DatabaseAccess.ConnectionString)
        );

        return builder;
    }
}
