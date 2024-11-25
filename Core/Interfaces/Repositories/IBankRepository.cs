using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankRepository
{
    Task<LoanRequestDto> AddLoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(Loan LoanApproved, List<Installment> installments);
    Task<LoanRejectDto> RejectLoan(LoanRequest loanRequest);
    Task<List<Installment>> GetInstallmentsOverdue();
    Task<TermInterestRate> GetMonthsByMonths(int months);
    Task PaidInstallments(List<Installment> installments);
    Task<LoanRequest> VerifyLoanRequest(int Id);
    Task<List<Installment>> VerifyExistsInstallmentsByLoanId(int LoanId);
    Task<List<Installment>> GetInstallmentsByLoanId(int Id, string? status);
    Task<Customer> VerifyCustomer(int Id);
    Task<Loan> GetLoanById(int Id);
}
