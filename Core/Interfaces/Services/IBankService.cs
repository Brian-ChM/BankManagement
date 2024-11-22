using Core.Request;

namespace Core.Interfaces.Services;

public interface IBankService
{
    Task<LoanApplicationRequest> LoanRequest(LoanApplicationRequest loanApplication);
}
