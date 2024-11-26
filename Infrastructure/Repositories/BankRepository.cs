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

    public async Task<LoanRequestDto> AddLoanRequest(LoanApplicationRequest loanApplication)
    {
        var termInterest = await GetMonthsByMonths(loanApplication.MonthRequest);

        var addLoanRequest = loanApplication.Adapt<LoanRequest>();
        addLoanRequest.TermInterestRateId = termInterest.Id;

        _context.Add(addLoanRequest);
        await _context.SaveChangesAsync();

        return addLoanRequest.Adapt<LoanRequestDto>();
    }

    public async Task<LoanApproveDto> ApproveLoan(Loan loanApproved, List<Installment> installments)
    {
        _context.Loans.Add(loanApproved);
        _context.Installments.AddRange(installments);
        await _context.SaveChangesAsync();

        return loanApproved.Adapt<LoanApproveDto>();
    }

    public async Task<LoanRejectDto> RejectLoan(LoanRequest loanRequest)
    {
        _context.LoanRequests.Update(loanRequest);
        await _context.SaveChangesAsync();

        return loanRequest.Adapt<LoanRejectDto>();
    }

    public async Task<List<Installment>> GetInstallmentsOverdue()
    {
        return await _context.Installments
            .Include(x => x.Loan)
            .ThenInclude(x => x.Customer)
            .Where(x => x.DueDate < DateTime.UtcNow && x.Status.ToLower() == "pending")
            .ToListAsync();
    }


    public async Task<TermInterestRate> GetMonthsByMonths(int months)
    {
        return await _context.TermInterestRates.FirstOrDefaultAsync(x => x.Months == months) ??
            throw new Exception("Seleccione un mes valido.");
    }

    public async Task PaidInstallments(List<Installment> installments)
    {
        var paidInstallments = installments.Where(x => x.Status.ToLower() == "paid").Select(x => new Payment
        {
            InstallmentId = x.Id,
            Amount = x.TotalAmount,
            PaymentDate = DateTime.UtcNow.Date,
            Status = x.Status,
        });

        _context.Payments.AddRange(paidInstallments);
        _context.Installments.UpdateRange(installments);
        await _context.SaveChangesAsync();
    }

    public async Task<LoanRequest> VerifyLoanRequest(int id)
    {
        return await _context.LoanRequests
            .Include(x => x.Customer)
            .Include(x => x.TermInterestRate).FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
            throw new Exception($"No se encontro la solicitud de prestamo con el Id {id}");
    }

    public async Task<List<Installment>> VerifyExistsInstallmentsByLoanId(int loanId)
    {
        var installments = await _context.Installments
            .Where(x => x.Status.ToLower() == "pending" && x.LoanId == loanId)
            .OrderBy(x => x.DueDate)
            .ToListAsync();

        return installments;
    }

    public async Task<List<Installment>> GetInstallmentsByLoanId(int id, string? status)
    {
        var query = _context.Installments
            .Include(x => x.Loan)
            .ThenInclude(x => x.Customer)
            .OrderBy(x => x.DueDate)
            .Where(x => x.Loan.Id == id);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status.ToLower() == status.ToLower());

        return await query.ToListAsync();
    }

    public async Task<Customer> VerifyCustomer(int id)
    {
        return await _context.Customers.FindAsync(id) ??
            throw new Exception($"No se encontro un cliente con el Id {id}");
    }

    public async Task<Loan> GetLoanById(int id)
    {
        return await _context.Loans
            .Include(x => x.Customer)
            .Include(x => x.Installments)
            .FirstOrDefaultAsync(x => x.Id == id) ??
            throw new Exception("El prestamo solicitado no existe.");
    }
}
