using VoltHub.Domain.Common;

namespace VoltHub.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = default!;

    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
    // Computed, not persisted — excluded via .Ignore() in the EF configuration.

    private RefreshToken() { }

    private RefreshToken(Guid id, string tokenHash, DateTime expiresAt, Guid userId)
    {
        Id = id;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    // Stores only a SHA-256 hash of the token the client receives, so a leaked row can't be replayed.
    public static RefreshToken Create(string tokenHash, DateTime expiresAt, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash is required.", nameof(tokenHash));
        if (expiresAt <= DateTime.UtcNow)
            throw new ArgumentException("Expiry must be in the future.", nameof(expiresAt));

        return new RefreshToken(Guid.CreateVersion7(), tokenHash, expiresAt, userId);
    }

    public void Revoke()
    {
        if (RevokedAt is not null)
            throw new InvalidOperationException("Token is already revoked.");

        RevokedAt = DateTime.UtcNow;
    }
}

