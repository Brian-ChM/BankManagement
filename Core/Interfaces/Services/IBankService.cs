using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Services;

public interface IBankService
{
    Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(int loanRequestId);
    Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject);
    Task<string> GetToken(int Id);
    List<Installment> GenerateInstallments(Loan loan);
}
