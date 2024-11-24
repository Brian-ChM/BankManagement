using Core.DTOs;
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

    public async Task<LoanRequestDto> AddLoanApplication(LoanApplicationRequest loanApplication)
    {
        var TermInterest = await GetMonthsByMonths(loanApplication.MonthRequest);

        var AddLoanRequest = loanApplication.Adapt<LoanRequest>();
        AddLoanRequest.TermInterestRateId = TermInterest.Id;

        _context.Add(AddLoanRequest);
        await _context.SaveChangesAsync();

        return new LoanRequestDto
        {
            Message = $"Solicitud lista, estado {AddLoanRequest.Status}"
        };
    }

    public async Task<LoanApproveDto> ApproveLoan(Loan LoanApproved, List<Installment> installments)
    {
        _context.Loans.Add(LoanApproved);
        _context.Installments.AddRange(installments);
        await _context.SaveChangesAsync();

        return LoanApproved.Adapt<LoanApproveDto>();
    }

    public async Task<LoanRejectDto> RejectLoan(LoanRequest loanRequest)
    {
        _context.LoanRequests.Update(loanRequest);
        await _context.SaveChangesAsync();

        return loanRequest.Adapt<LoanRejectDto>();
    }

    public async Task<TermInterestRate> GetMonthsByMonths(int Months)
    {
        return await _context.TermInterestRates.FirstOrDefaultAsync(x => x.Months == Months) ??
            throw new Exception("Seleccione un mes valido.");
    }

    public async Task<Customer> VerifyCustomer(int Id)
    {
        return await _context.Customers.FindAsync(Id) ??
            throw new Exception($"No se encontro un cliente con el Id {Id}");
    }

    public async Task<LoanRequest> VerifyLoanRequest(int Id)
    {
        return await _context.LoanRequests
            .Include(x => x.Customer)
            .Include(x => x.TermInterestRate).FirstOrDefaultAsync(x => x.Id.Equals(Id)) ??
            throw new Exception($"No se encontro la solicitud de prestamo con el Id {Id}");
    }

    public async Task<Loan> GetLoanById (int Id)
    {
        return await _context.Loans
            .Include(x => x.Customer)
            .Include(x => x.Installments)
            .FirstOrDefaultAsync(x => x.Id == Id) ??
            throw new Exception("El prestamo solicitado no existe.");
    }
}
