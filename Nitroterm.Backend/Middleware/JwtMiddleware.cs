using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Nitroterm.Backend.Database;
using Nitroterm.Backend.Database.Models;
using Nitroterm.Backend.Dto;
using Nitroterm.Backend.Utilities;

namespace Nitroterm.Backend.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task Invoke(HttpContext context)
    {
        string[]? authParts = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ");
        //if (authParts == null || authParts[0] != "Bearer") return;

        if (authParts != null && authParts.Length == 2)
        {
            if (!await ValidateToken(context, authParts[1])) return;
        }

        await _next(context);
    }

    async Task<bool> ValidateToken(HttpContext context, string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secrets.Instance.JwtKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            JwtSecurityToken jwt = (JwtSecurityToken)validatedToken;
            int userId = int.Parse(jwt.Claims.First(x => x.Type == "uid").Value);

            using NitrotermDbContext db = new();

            User? user = db.GetUser(userId);
            if (user == null) return false;
            if (!user.IsTokenJtiValid(jwt.Payload.Jti)) return false;
            
            context.Items["User"] = user;

            return true;
        }
        catch (Exception e)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new ErrorResultDto("invalid_token", 
                "provided token is invalid or expired"));

            return false;
        }
    }
}