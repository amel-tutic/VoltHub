using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace VoltHub.Infrastructure.Persistence;

// Applies any migrations the database doesn't have yet. On an empty database (the hosted demo)
// this creates the whole schema, including the btree_gist extension and the overlap constraints.
public static class DatabaseMigrator
{
    public static async Task MigrateAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VoltHubDbContext>();
        await db.Database.MigrateAsync(cancellationToken);
    }
}
