using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankRepository
{
    Task<TermInterestRate> GetMonthsByMonths(int Months);
    Task<LoanRequestDto> AddLoanApplication(LoanApplicationRequest loanApplication);

    Task<LoanApproveDto> ApproveLoan(int LoanRequestId);
}
