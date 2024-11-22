using Core.Interfaces.Repositories;
using Core.Request;
using Infrastructure.Context;

namespace Infrastructure.Repositories;

public class BankMangmanetRepository : IBankMangmanetRepository
{
    private readonly AppDbContext _context;

    public BankMangmanetRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<LoanRequestDto> LoanRequest(LoanRequestDto loanRequest)
    {
        throw new NotImplementedException();
    }
}
