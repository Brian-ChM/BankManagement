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
        var Handler = new JwtSecurityTokenHandler();
        var Secret = Encoding.UTF8.GetBytes(JwtConfig.Secret);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Secret),
            SecurityAlgorithms.HmacSha256);


        // Agregar validación si no existe el customer con el id
        var Claims = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Name, customer.FirstName),
            new Claim(ClaimTypes.Role, customer.Role)
        ]);

        var TokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(10),
            Subject = Claims
        };

        var TokenHandler = Handler.CreateToken(TokenDescriptor);
        return Handler.WriteToken(TokenHandler);
    }
}
