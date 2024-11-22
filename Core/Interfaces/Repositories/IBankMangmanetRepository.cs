using Core.Request;

namespace Core.Interfaces.Repositories;

public interface IBankMangmanetRepository
{
    Task<LoanRequestDto> LoanRequest(LoanRequestDto loanRequest);
}
