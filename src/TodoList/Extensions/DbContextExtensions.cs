using Microsoft.EntityFrameworkCore;
using TodoList.Constants;
using TodoList.Contexts;

namespace TodoList.Extensions;

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
