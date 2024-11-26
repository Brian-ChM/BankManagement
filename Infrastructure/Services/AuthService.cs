using Core.DTOs;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    public string CreateToken(CustomerDto customer)
    {
        var handler = new JwtSecurityTokenHandler();
        var secret = Encoding.UTF8.GetBytes(JwtConfig.Secret);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(secret),
            SecurityAlgorithms.HmacSha256);

        var claims = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Name, customer.FirstName),
            new Claim(ClaimTypes.Role, customer.Role)
        ]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMonths(1),
            Subject = claims
        };

        var tokenHandler = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(tokenHandler);
    }
}
