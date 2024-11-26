using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ILoanRepository _loanRepository;

    public AuthService(ILoanRepository loanRepository)
    {
        _loanRepository = loanRepository;
    }
    public async Task<string> CreateToken(int id)
    {
        var customer = await _loanRepository.VerifyCustomer(id);
        var customerDto = customer.Adapt<CustomerDto>();

        var handler = new JwtSecurityTokenHandler();
        var secret = Encoding.UTF8.GetBytes(JwtConfig.Secret);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(secret),
            SecurityAlgorithms.HmacSha256);

        var claims = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, customerDto.Id.ToString()),
            new Claim(ClaimTypes.Name, customerDto.FirstName),
            new Claim(ClaimTypes.Role, customerDto.Role)
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
