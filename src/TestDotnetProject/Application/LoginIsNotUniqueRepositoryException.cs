namespace TestDotnetProject.Application;

public class LoginIsNotUniqueRepositoryException : Exception
{
    public LoginIsNotUniqueRepositoryException() { }

    public LoginIsNotUniqueRepositoryException(string message) : base(message) { }

    public LoginIsNotUniqueRepositoryException(string message, Exception innerException) : base(message, innerException)
    { }

    public LoginIsNotUniqueRepositoryException(string message, string login) : this(message)
    {
        Login = login;
    }

    public string? Login { get; }
}
