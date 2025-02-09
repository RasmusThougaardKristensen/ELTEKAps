using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ELTEKAps.Management.Infrastructure.Extensions;
public static class ApplicationBuilderExtension
{
    public static void EnsureDatabaseMigrated(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<TaskContext>();

        context.Database.Migrate();
    }
}