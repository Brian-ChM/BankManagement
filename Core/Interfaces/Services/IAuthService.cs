namespace Core.Interfaces.Services;

public interface IAuthService
{
    Task<string> CreateToken(int id);
}
