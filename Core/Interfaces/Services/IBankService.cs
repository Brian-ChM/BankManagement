using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Services;

public interface IBankService
{
    Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(int loanRequestId);
    Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject);
    Task<LoanDetailedDto> DetailedLoan(int Id);
    Task<string> GetToken(int Id);
    Task<List<InstallmentsDto>> GetInstallments(int Id, string? status);
    Task<List<InstallmentsOverdueDto>> GetInstallmentsOverdue();
    List<Installment> GenerateInstallments(Loan loan);
}
