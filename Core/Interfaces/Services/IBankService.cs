using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Services;

public interface IBankService
{
    Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(int loanRequestId);
    Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject);
    Task<LoanDetailedDto> DetailedLoan(int id);
    Task<string> PaidInstallments(InstallmentPaymentRequest installmentPayment);
    Task<List<InstallmentsDto>> GetInstallments(int id, string? status);
    Task<List<InstallmentsOverdueDto>> GetInstallmentsOverdue();

    List<Installment> GenerateInstallments(Loan loan);
    string GetLoanType(string value);
    Task<string> GetToken(int id);
}
