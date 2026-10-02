using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoltHub.Application.Common.Interfaces;
using VoltHub.Domain.Entities;
using VoltHub.Domain.Enums;

namespace VoltHub.Infrastructure.Persistence;

// Development only: staff accounts for every role, plus a small charging network to demo with.
public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<VoltHubDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await SeedStaffAsync(db, passwordHasher, configuration);
        await SeedStationsAsync(db);
    }

    private static async Task SeedStaffAsync(VoltHubDbContext db, IPasswordHasher passwordHasher, IConfiguration configuration)
    {
        if (await db.Users.AnyAsync(u => u.Role != UserRole.User))
            return;   // staff accounts already exist

        var password = configuration["DevSeed:Password"]
            ?? throw new InvalidOperationException("DevSeed:Password is not configured.");

        db.Users.AddRange(
            User.Create("Admin", "VoltHub", "admin@volthub.local", passwordHasher.Hash(password), UserRole.Admin),
            User.Create("Operator", "VoltHub", "operator@volthub.local", passwordHasher.Hash(password), UserRole.Operator));
        await db.SaveChangesAsync();
    }

    private static async Task SeedStationsAsync(VoltHubDbContext db)
    {
        if (await db.ChargingStations.AnyAsync())
            return;

        var centar = ChargingStation.Create("VoltHub Centar", "Trg slobode 1", "Novi Sad", 45.2551, 19.8451, "Garaža u centru grada");
        var liman = ChargingStation.Create("VoltHub Liman", "Bulevar cara Lazara 5", "Novi Sad", 45.2461, 19.8411);
        var usce = ChargingStation.Create("VoltHub Ušće", "Bulevar Mihajla Pupina 4", "Beograd", 44.8157, 20.4368, "Parking tržnog centra");
        db.ChargingStations.AddRange(centar, liman, usce);

        var brokenCharger = Charger.Create("B2", ConnectorType.Type2, CurrentType.AC, 22m, 25m, liman.Id);
        brokenCharger.SetStatus(ChargerStatus.OutOfOrder);   // lets the demo show a charger that can't be reserved

        db.Chargers.AddRange(
            Charger.Create("A1", ConnectorType.CCS, CurrentType.DC, 50m, 35m, centar.Id),
            Charger.Create("A2", ConnectorType.Type2, CurrentType.AC, 22m, 25m, centar.Id),
            Charger.Create("A3", ConnectorType.CHAdeMO, CurrentType.DC, 50m, 35m, centar.Id),
            Charger.Create("B1", ConnectorType.CCS, CurrentType.DC, 150m, 45m, liman.Id),
            brokenCharger,
            Charger.Create("C1", ConnectorType.CCS, CurrentType.DC, 100m, 40m, usce.Id),
            Charger.Create("C2", ConnectorType.Tesla, CurrentType.DC, 150m, 45m, usce.Id));
        await db.SaveChangesAsync();
    }
}
