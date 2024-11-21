using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankMangmanetRepository
{
    Task<LoanRequest> LoanRequest(LoanRequest loanRequest);
}
