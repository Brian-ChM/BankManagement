using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankRepository
{
    Task<LoanRequestDto> AddLoanApplication(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(Loan LoanApproved, List<Installment> installments);
    Task<LoanRejectDto> RejectLoan(LoanRequest loanRequest);
    Task<TermInterestRate> GetMonthsByMonths(int months);
    Task<List<Installment>> VerifyExistsInstallmentsByLoanId(int LoanId);
    Task<Customer> VerifyCustomer(int Id);
    Task<List<Installment>> GetInstallmentsByLoanId(int Id, string? status);
    Task<List<Installment>> GetInstallmentsOverdue();
    Task<LoanRequest> VerifyLoanRequest(int Id);
    Task<Loan> GetLoanById(int Id);
}
