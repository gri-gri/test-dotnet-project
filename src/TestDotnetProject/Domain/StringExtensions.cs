using System.Text.RegularExpressions;

namespace TestDotnetProject.Domain;

public static class StringExtensions
{
    private static readonly Regex AlphanumericRegex = new("^[a-z0-9]*$", RegexOptions.IgnoreCase);
    private static readonly Regex LatinRussianLettersRegex = new("^[a-zА-Я]*$", RegexOptions.IgnoreCase);

    public static bool IsAlphaNumeric(this string s) => AlphanumericRegex.IsMatch(s);
    public static bool ConsistsFromLatinRussianLetters(this string s) => LatinRussianLettersRegex.IsMatch(s);
}
