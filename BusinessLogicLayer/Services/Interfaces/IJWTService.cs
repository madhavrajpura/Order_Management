using System.Security.Claims;

namespace BusinessLogicLayer.Services.Interfaces;

public interface IJWTService
{
    string GenerateToken(string email,string roleName,int UserId);
    ClaimsPrincipal? GetClaimsFromToken(string token);
    string? GetClaimValue(string token, string claimType);
}
