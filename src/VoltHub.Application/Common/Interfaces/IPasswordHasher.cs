namespace VoltHub.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password);

    // passwordHash is null when the account doesn't exist; the check still takes the same time.
    bool Verify(string password, string? passwordHash);
}
