namespace TestDotnetProject.Application;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() { }

    public UserNotFoundException(string message) : base(message) { }

    public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
    { }

    public UserNotFoundException(string message, string login) : this(message)
    {
        Login = login;
    }

    public string? Login { get; }
}
