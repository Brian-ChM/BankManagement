using Core.DTOs;
using Core.Request;

namespace Core.Interfaces.Services;

public interface ILoanService
{
    Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(int loanRequestId);
    Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject);
    Task<LoanDetailedDto> DetailedLoan(int id);
    string GetLoanType(string value);
}
