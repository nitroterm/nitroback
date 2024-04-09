namespace Nitroterm.Backend.Utilities;

public static class ReCaptcha
{
    public static async Task<bool> VerifyAsync(string userChallenge)
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Add("User-Agent", "Nitroterm.Backend");
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"secret", Secrets.Instance.ReCaptcha },
            {"response", userChallenge }
        });
        
        var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Response>();
            
            return result.Success;
        }

        return false;
    }

    public static bool Verify(string userChallenge)
        => VerifyAsync(userChallenge).GetAwaiter().GetResult();
    
    public class Response
    {
        public bool Success { get; set; }
        public string[] ErrorCodes { get; set; }
    }
}