using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Database.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public Product? Product { get; set; }
    public List<Token> Tokens { get; set; }
    public int NitroLevel { get; set; }

    public bool IsTokenJtiValid(string jti)
        => Tokens.Any(token => token.Value == jti);

    public void SetPassword(string password)
    {
        (Password, Salt) = SecurityUtilities.PasswordHash(password);
    }

    public bool CheckPassword(string password)
        => SecurityUtilities.VerifyPassword(password, Password, Salt);
    
    public string IssueJwtToken(NitrotermDbContext db)
    {
        string tokenJti = Guid.NewGuid().ToString();
        
        Claim[] claims =
        {
            new(JwtRegisteredClaimNames.Iss, "nitroterm"),
            new(JwtRegisteredClaimNames.Jti, tokenJti),
            new("uid", Id.ToString()),
            new("username", Username)
        };
        
        db.AddToken(this, tokenJti);

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secrets.Instance.JwtKey));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        JwtSecurityToken token = new JwtSecurityToken(
            "nitroterm",
            null,
            claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}