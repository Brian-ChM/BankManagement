using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankRepository
{
    Task<LoanRequestDto> AddLoanApplication(LoanApplicationRequest loanApplication);
    Task<CustomerDto> GetToken(int Id);
    Task<Loan> ApproveLoan(int loanRequestId);
    Task AddInstallments(List<Installment> installments);
    Task<TermInterestRate> GetMonthsByMonths(int months);
    Task<Customer> VerifyCustomer(int Id);
    Task<LoanRequest> VerifyLoanRequest(int Id);

}
