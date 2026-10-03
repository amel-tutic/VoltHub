using VoltHub.Domain.Enums;

namespace VoltHub.Application.Reports;

// SSA 9.1: users and vehicles, plus the size of the network.
public sealed record OverviewReportResponse(
    int Owners, int ActiveOwners, int Operators, int Admins, int Vehicles,
    int Stations, int Chargers, int ChargersOutOfOrder, int ActiveReservations, int CompletedSessions);

// SSA 9.2: how busy each station was in the period. Utilization = charging hours / (chargers x hours in period).
public sealed record StationUsageReportResponse(
    Guid StationId, string StationName, string City, int ChargerCount, int Sessions, decimal EnergyKwh, decimal Revenue,
    double AverageSessionMinutes, double UtilizationPercent);

// SSA 9.3: consumption and money in the period.
public sealed record RevenueByDayResponse(DateOnly Day, int Sessions, decimal EnergyKwh, decimal Revenue);

public sealed record RevenueReportResponse(
    DateTime From, DateTime To, int Sessions, decimal EnergyKwh, decimal Billed, decimal Paid, decimal Outstanding,
    IReadOnlyList<RevenueByDayResponse> ByDay);

// SSA 9.4: how often each charger failed in the period.
public sealed record FaultReportResponse(
    Guid ChargerId, string ChargerCode, string StationName, ChargerStatus Status, int Faults, int OpenFaults, DateTime? LastFaultAt);
