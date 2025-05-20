namespace TestDotnetProject;

public class User
{
    public User(
        string login, string password, string name, int gender, DateTime? birthday, bool isAdmin, string creatorLogin)
    {
        Guid = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;

        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        Birthday = birthday;
        Admin = isAdmin;
        CreatedBy = creatorLogin;
        ModifiedBy = creatorLogin;
    }

    public Guid Guid { get; }
    public string Login { get; }
    public string Password { get; } // Storing plain-text password is required by task
    public string Name { get; }
    public int Gender { get; }
    public DateTime? Birthday { get; }
    public bool Admin { get; }
    public DateTime CreatedOn { get; }
    public string CreatedBy { get; }
    public DateTime ModifiedOn { get; }
    public string ModifiedBy { get; }
    public DateTime? RevokedOn { get; } = default;
    public string? RevokedBy { get; } = default;
}
