using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Infrastructure.Persistence;

// Development only: creates one administrator and one operator so every role can be demonstrated.
public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VoltHubDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        if (await db.Users.AnyAsync(u => u.Role != UserRole.User))
            return;   // staff accounts already exist

        var password = configuration["DevSeed:Password"]
            ?? throw new InvalidOperationException("DevSeed:Password is not configured.");

        db.Users.AddRange(
            User.Create("Admin", "VoltHub", "admin@volthub.local", passwordHasher.Hash(password), UserRole.Admin),
            User.Create("Operator", "VoltHub", "operator@volthub.local", passwordHasher.Hash(password), UserRole.Operator));
        await db.SaveChangesAsync();
    }
}
