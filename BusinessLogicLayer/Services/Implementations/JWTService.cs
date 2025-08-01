using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessLogicLayer.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogicLayer.Services.Implementations;

public class JWTService : IJWTService
{
    private readonly string _secretKey;
    private readonly int _tokenDuration;

    public JWTService(IConfiguration configuration)
    {
        _secretKey = configuration.GetValue<string>("JwtConfig:Key")!;
        _tokenDuration = configuration.GetValue<int>("JwtConfig:Duration");
    }

    public string GenerateToken(string email, string roleName, int UserId)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
                new Claim("email", email),
                new Claim("role", roleName),
                new Claim(ClaimTypes.NameIdentifier,UserId.ToString())
            };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateResetToken(string email, string password)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = new[]
        {
                new Claim("email", email),
                new Claim("password", password)
            };

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: "localhost",
            audience: "localhost",
            claims: claims,
            expires: DateTime.Now.AddHours(_tokenDuration),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? GetClaimsFromToken(string token)
    {
        JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        JwtSecurityToken? jwtToken = handler.ReadJwtToken(token);
        ClaimsIdentity claims = new ClaimsIdentity(jwtToken.Claims);
        return new ClaimsPrincipal(claims);
    }

    public string? GetClaimValue(string token, string claimType)
    {
        ClaimsPrincipal? claimsPrincipal = GetClaimsFromToken(token);
        string? value = claimsPrincipal?.FindFirst(claimType)?.Value;
        return value;
    }

}
