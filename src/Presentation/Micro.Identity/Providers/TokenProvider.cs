using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Micro.Core;
using Micro.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Micro.Identity.Providers;

public class TokenProvider
{
    private readonly IConfiguration _configuration;

    public TokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public (string Token, long ExpiresInSeconds) CreateToken(IdentityUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        var securityKey = new SymmetricSecurityKey(key);
        var expires = DateTime.UtcNow.AddHours(1);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Expiration, expires.ToString(CultureInfo.InvariantCulture)),
            ]),
            Expires = expires,
            SigningCredentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), Convert.ToInt64((expires - DateTime.Now).TotalSeconds));
    }

    public (string Token, long ExpiresInSeconds) CreateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
        var securityKey = new SymmetricSecurityKey(key);
        var expires = DateTime.UtcNow.AddHours(1);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Expiration, expires.ToString(CultureInfo.InvariantCulture)),
                new Claim("IsActive", user.IsActive.ToString())
            ]),
            Expires = expires,
            SigningCredentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), Convert.ToInt64((expires - DateTime.Now).TotalSeconds));
    }
}