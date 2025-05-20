namespace TestDotnetProject.Domain;

public class User
{
    private string login;
    private string password; // Storing plain-text password is required by task
    private string name;
    private int gender;

    private User() { } // Used by EF Core

    public User(
        string login, string password, string name, int gender, DateTime? birthday, bool isAdmin, string creatorLogin)
    {
        Guid = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;

        Login = login;
        Password = password;
        Name = name;
        Gender = gender;
        Birthday = birthday;
        Admin = isAdmin;
        CreatedBy = creatorLogin;

        MarkModified(creatorLogin);
    }

    public Guid Guid { get; private set; }

    public string Login
    {
        get => login;
        set
        {
            if (!value.IsAlphaNumeric())
            {
                throw new ArgumentException($"{value} is not alphanumeric", nameof(Login));
            }

            login = value;
        }

    }

    public string Password
    {
        get => password;
        set
        {
            if (!value.IsAlphaNumeric())
            {
                throw new ArgumentException($"{value} is not alphanumeric", nameof(Password));
            }

            password = value;
        }
    }

    public string Name
    {
        get => name;
        set
        {
            if (!value.ConsistsFromLatinRussianLetters())
            {
                throw new ArgumentException(
                    $"{value} does not consist from latin and/or russian letters", nameof(Name));
            }

            name = value;
        }
    }

    public int Gender
    {
        get => gender;
        set
        {
            if (value < 0 || value > 2)
            {
                throw new ArgumentException($"{value} is not 0, 1 or 2", nameof(Gender));
            }

            gender = value;
        }
    }


    public DateTime? Birthday { get; private set; }
    public bool Admin { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime ModifiedOn { get; private set; }
    public string ModifiedBy { get; private set; }
    public DateTime? RevokedOn { get; private set; } = default;
    public string? RevokedBy { get; private set; } = default;

    public void Revoke(string revokerLogin)
    {
        RevokedOn = DateTime.UtcNow;
        RevokedBy = revokerLogin;

        MarkModified(revokerLogin);
    }

    public void Revive()
    {
        RevokedOn = default;
        RevokedBy = default;
    }

    private void MarkModified(string modifierLogin)
    {
        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifierLogin;
    }
}
