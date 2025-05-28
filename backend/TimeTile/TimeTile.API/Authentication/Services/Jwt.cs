using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TimeTile.API.Authentication.Services;

public class JwtOptions
{
    public required string Key { get; init; }
    public string? Issuer { get; init; }
    public string? Audience { get; init; }
}

public class Jwt
{
    private readonly JwtOptions _options;

    public Jwt(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var key = SecurityKey(_options.Key);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddYears(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public static SymmetricSecurityKey SecurityKey(string key) => new(Encoding.UTF8.GetBytes(key));
}