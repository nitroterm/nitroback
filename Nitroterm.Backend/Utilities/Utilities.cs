namespace Nitroterm.Backend.Utilities;

public class Utilities
{
    public static bool CheckUsername(string username)
        => CheckUserContent(username)
           && username == username.ToLower()
           && username.Length < 30
           && !username.Contains(' ')
           && username.All(c => char.IsAsciiLetterOrDigit(c) || c == '_' && c != '@');
    
    public static bool CheckDisplayName(string username)
        => CheckUserContent(username)
           && username.Length < 30;

    public static bool CheckPassword(string password)
        => CheckUserContent(password)
           && password.Length >= 5
           && password.Length < 100
           && password.Any(char.IsDigit)
           && password.Any(char.IsLetter);

    public static bool CheckUserContent(string userContent)
        => userContent.Length < 4000
           && !string.IsNullOrWhiteSpace(userContent);
}