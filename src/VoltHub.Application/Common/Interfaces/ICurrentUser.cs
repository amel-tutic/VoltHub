namespace VoltHub.Application.Common.Interfaces;

// The logged-in user of the current request (read from the JWT by the Api project).
public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsInRole(string role);
}
