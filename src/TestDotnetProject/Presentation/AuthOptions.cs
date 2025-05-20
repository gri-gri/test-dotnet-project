using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TestDotnetProject.Presentation;

public static class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
    public static readonly string KEY = Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? throw new InvalidOperationException("Environment variable 'JWT_SECRET' must be set");
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.UTF8.GetBytes(KEY));
}
