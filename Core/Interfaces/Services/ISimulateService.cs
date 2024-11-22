using Core.DTOs;
using Core.Request;

namespace Core.Interfaces.Services;

public interface ISimulateService
{
    Task<LoanDto> SimulateLoan(LoanSimulateRequest request);
}