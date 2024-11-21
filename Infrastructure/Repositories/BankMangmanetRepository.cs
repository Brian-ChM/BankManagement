using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Request;
using Infrastructure.Context;
using Mapster;

namespace Infrastructure.Repositories;

public class BankMangmanetRepository : IBankMangmanetRepository
{
    private readonly AppDbContext _context;

    public BankMangmanetRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<LoanRequest> LoanRequest(LoanRequest loanRequest)
    {
        var AddLoanRequest = loanRequest.Adapt<Loan>();

        throw new NotImplementedException();
    }
}
