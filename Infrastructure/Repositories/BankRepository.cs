using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Request;
using Infrastructure.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BankRepository : IBankRepository
{
    private readonly AppDbContext _context;

    public BankRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<LoanApplicationRequest> LoanRequest(LoanApplicationRequest loanApplication)
    {
        throw new NotImplementedException();
    }

    public async Task<LoanApplicationRequest> AddLoanApplication(LoanApplicationRequest loanApplication)
    {

        var Customer = await VerifyExists(loanApplication.CustomerId);
        var TermInterest = await GetMonthsByMonths(loanApplication.MonthRequest);

        var AddLoanRequest = loanApplication.Adapt<LoanRequest>();
        AddLoanRequest.TermInterestRateId = TermInterest.Id; 

        _context.Add(AddLoanRequest);
        await _context.SaveChangesAsync();

        return loanApplication;
    }

    public async Task<TermInterestRate> GetMonthsByMonths(int Months)
    {
        return await _context.TermInterestRates.FirstOrDefaultAsync(x => x.Months == Months) ??
            throw new Exception("Seleccione un mes valido.");
    }

    public async Task<Customer> VerifyExists (int Id)
    {
        return await _context.Customers.FindAsync(Id) ??
            throw new Exception($"No se encontro un cliente con el Id {Id}");
    }


}
