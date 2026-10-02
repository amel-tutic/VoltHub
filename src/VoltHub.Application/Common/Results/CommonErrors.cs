namespace VoltHub.Application.Common.Results;

public static class CommonErrors
{
    public static readonly Error NotAuthenticated = Error.Unauthorized("Auth.NotAuthenticated", "You are not logged in.");
}
