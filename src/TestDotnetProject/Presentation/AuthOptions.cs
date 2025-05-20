using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace TestDotnetProject.Presentation;

public static class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
    public const string KEY = ";alsdfjk;lasdjkf;laskdjnvwaeoi32090j9slkanvlfjp9ejpo";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new(Encoding.UTF8.GetBytes(KEY));
}
