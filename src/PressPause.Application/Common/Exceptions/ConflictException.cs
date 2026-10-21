namespace PressPause.Application.Common.Exceptions;

/// <summary>
/// Thrown when a request cannot be completed because it conflicts with the
/// current state of a resource (for example, adding something that already
/// exists). The API maps this to HTTP 409.
/// </summary>
/// <remarks>
/// Derives from <see cref="InvalidOperationException"/> so existing code that
/// treats these cases as invalid operations keeps working.
/// </remarks>
public sealed class ConflictException : InvalidOperationException
{
    public ConflictException(string message)
        : base(message)
    {
    }
}
