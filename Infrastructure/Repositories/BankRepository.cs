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

    public async Task<CustomerDto> GetToken(int Id)
    {
        var Customer = await _context.Customers.FindAsync(Id);
        return Customer.Adapt<CustomerDto>();
    }
    public async Task<LoanRequestDto> AddLoanApplication(LoanApplicationRequest loanApplication)
    {

        var Customer = await VerifyCustomer(loanApplication.CustomerId);
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

    public async Task<Loan> ApproveLoan(int LoanRequestId)
    {
        var LoanRequest = await VerifyLoanRequest(LoanRequestId);
        var Approve = LoanRequest.Adapt<Loan>();
        LoanRequest.Status = "Approve";

        _context.Loans.Add(Approve);
        await _context.SaveChangesAsync();

        return Approve;
    }

    public async Task AddInstallments(List<Installment> installments)
    {
        _context.Installments.AddRange(installments);
        await _context.SaveChangesAsync();
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
}
