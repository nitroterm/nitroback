using System.Text.Json;

namespace Nitroterm.Backend.Utilities;

public static class Secrets
{
    public static SecretsInstance Instance { get; private set; }

    public static void Load()
    {
        if (!File.Exists("secrets.json"))
        {
            throw new Exception(
                "No secrets file. Please create a secrets file next to the program " +
                "in order to be able to connect to the database and generate JWT tokens");
        }

        Instance = JsonSerializer.Deserialize<SecretsInstance>(File.ReadAllText("secrets.json"))!;
    }
}

public class SecretsInstance
{
    public string ConnectionString { get; set; }
    public string JwtKey { get; set; }
    public string ReCaptcha { get; set; }
    public string Firebase { get; set; }
}