using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InstallmentRepository : IInstallmentRepository
{
    private readonly AppDbContext _context;

    public InstallmentRepository(AppDbContext context)
    {
        _context = context;
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


}
