namespace TestDotnetProject.Application.Exceptions;

public class LoginIsNotUniqueException : Exception
{
    public LoginIsNotUniqueException() { }

    public LoginIsNotUniqueException(string message) : base(message) { }

    public LoginIsNotUniqueException(string message, Exception innerException) : base(message, innerException)
    { }

    public LoginIsNotUniqueException(string message, string login) : this(message)
    {
        Login = login;
    }

    public string? Login { get; }

    public static LoginIsNotUniqueException FromLogin(string login)
    {
        return new LoginIsNotUniqueException("Login is not unique", login);
    }
}
