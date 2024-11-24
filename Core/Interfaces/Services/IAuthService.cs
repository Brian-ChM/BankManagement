using Core.DTOs;

namespace Core.Interfaces.Services;

public interface IAuthService
{
    string CreateToken(CustomerDto customer);
    //bool ValidateJwt(string token);
}
