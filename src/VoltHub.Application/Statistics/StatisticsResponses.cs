namespace VoltHub.Application.Statistics;

public sealed record StationUsageResponse(Guid StationId, string StationName, int Sessions, decimal EnergyKwh);

public sealed record MonthlyUsageResponse(string Month, int Sessions, decimal EnergyKwh, decimal Cost);

public sealed record MyStatisticsResponse(
    int SessionCount, decimal TotalEnergyKwh, decimal TotalCost, double AverageSessionMinutes,
    IReadOnlyList<StationUsageResponse> TopStations, IReadOnlyList<MonthlyUsageResponse> Monthly);
