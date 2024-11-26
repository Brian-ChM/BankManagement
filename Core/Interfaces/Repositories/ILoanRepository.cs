using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Repositories;

public interface ILoanRepository
{
    Task<LoanRequestDto> AddLoanRequest(LoanApplicationRequest loanApplication);
    Task<LoanApproveDto> ApproveLoan(Loan loanApproved, List<Installment> installments);
    Task<LoanRejectDto> RejectLoan(LoanRequest loanRequest);
    Task<LoanRequest> VerifyLoanRequest(int id);
    Task<Customer> VerifyCustomer(int id);
    Task<Loan> GetLoanById(int id);
}
