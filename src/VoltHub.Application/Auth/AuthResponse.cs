namespace VoltHub.Application.Auth;

public sealed record AuthResponse(string AccessToken, DateTime AccessTokenExpiresAt, string RefreshToken, string Role);
