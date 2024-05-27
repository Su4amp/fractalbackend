namespace Application.Common.Exceptions;

public class LockedException : Exception
{
    public LockedException()
    {
    }

    public LockedException(string message) : base(message)
    {
    }

    public LockedException(string message, Exception? innerException) : base(message, innerException)
    {
    }
}