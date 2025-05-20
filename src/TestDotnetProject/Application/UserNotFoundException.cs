namespace TestDotnetProject.Application;

public class UserNotFoundException : Exception
{
    public UserNotFoundException() { }

    public UserNotFoundException(string message) : base(message) { }

    public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
    { }

    public UserNotFoundException(string message, string id) : this(message)
    {
        Id = id;
    }

    public string? Id { get; }

    public static UserNotFoundException FromAnyStringId(string id)
    {
        return new UserNotFoundException("User was not found", id);
    }
}
