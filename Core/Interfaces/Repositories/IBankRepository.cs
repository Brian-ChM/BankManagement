using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankRepository
{
    Task<LoanRequestDto> AddLoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(Loan loanApproved, List<Installment> installments);
    Task<LoanRejectDto> RejectLoan(LoanRequest loanRequest);
    Task<List<Installment>> GetInstallmentsOverdue();
    Task<TermInterestRate> GetMonthsByMonths(int months);
    Task PaidInstallments(List<Installment> installments);
    Task<LoanRequest> VerifyLoanRequest(int id);
    Task<List<Installment>> VerifyExistsInstallmentsByLoanId(int loanId);
    Task<List<Installment>> GetInstallmentsByLoanId(int id, string? status);
    Task<Customer> VerifyCustomer(int id);
    Task<Loan> GetLoanById(int id);
}
