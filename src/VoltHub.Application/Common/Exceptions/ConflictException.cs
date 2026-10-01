namespace VoltHub.Application.Common.Exceptions;

// Thrown when the database itself rejects a write that conflicts with existing data
// (a taken email, an overlapping reservation) — typically two requests racing each other.
public sealed class ConflictException(string message, Exception? innerException = null)
    : Exception(message, innerException);
