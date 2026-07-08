using VoltHub.Domain.Common;

namespace VoltHub.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
    // Computed, not persisted — excluded via .Ignore() in the EF configuration.

    private RefreshToken() { }

    private RefreshToken(Guid id, string token, DateTime expiresAt, Guid userId)
    {
        Id = id;
        Token = token;
        ExpiresAt = expiresAt;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public static RefreshToken Create(string token, DateTime expiresAt, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token is required.", nameof(token));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiry must be in the future.", nameof(expiresAt));

        return new RefreshToken(Guid.NewGuid(), token, expiresAt, userId);
    }

    public void Revoke()
    {
        if (RevokedAt is not null)
            throw new InvalidOperationException("Token is already revoked.");

        RevokedAt = DateTime.UtcNow;
    }
}