using Core.DTOs;
using Core.Request;

namespace Core.Interfaces.Services;

public interface IBankService
{
    Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(int LoanRequestId);
}
