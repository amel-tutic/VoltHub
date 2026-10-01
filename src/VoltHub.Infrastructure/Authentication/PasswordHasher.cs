using VoltHub.Application.Common.Interfaces;

namespace VoltHub.Infrastructure.Authentication;

internal sealed class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;   // 2^12 rounds: slow enough to resist brute force, fast enough for login

    // Checked when an email isn't registered, so that case takes as long as a wrong password.
    private static readonly string DummyHash = BCrypt.Net.BCrypt.HashPassword("timing-equalizer", WorkFactor);

    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string? passwordHash)
    {
        if (passwordHash is null)
        {
            BCrypt.Net.BCrypt.Verify(password, DummyHash);
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
