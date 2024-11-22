using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankRepository
{
    Task<LoanApplicationRequest> LoanRequest(LoanApplicationRequest loanApplication);
    Task<TermInterestRate> GetMonthsByMonths(int Months);
    Task<LoanApplicationRequest> AddLoanApplication(LoanApplicationRequest loanApplication);
}
