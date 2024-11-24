﻿using Core.DTOs;
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
        var TermInterest = await GetMonthsByMonths(loanApplication.MonthRequest);

        var AddLoanRequest = loanApplication.Adapt<LoanRequest>();
        AddLoanRequest.TermInterestRateId = TermInterest.Id;

        _context.Add(AddLoanRequest);
        await _context.SaveChangesAsync();

        return AddLoanRequest.Adapt<LoanRequestDto>();
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

    public async Task<List<Installment>> GetInstallmentsOverdue()
    {
        return await _context.Installments
            .Include(x => x.Loan)
            .ThenInclude(x => x.Customer)
            .Where(x => x.DueDate < DateTime.UtcNow)
            .ToListAsync();
    }


    public async Task<TermInterestRate> GetMonthsByMonths(int Months)
    {
        return await _context.TermInterestRates.FirstOrDefaultAsync(x => x.Months == Months) ??
            throw new Exception("Seleccione un mes valido.");
    }

    public async Task PaidInstallments(List<Installment> installments)
    {
        var PaidInstallments = installments.Where(x => x.Status.ToLower() == "paid").Select(x => new Payment
        {
            InstallmentId = x.Id,
            Amount = x.TotalAmount,
            PaymentDate = DateTime.UtcNow.Date,
            Status = x.Status,
        });

        _context.Payments.AddRange(PaidInstallments);
        _context.Installments.UpdateRange(installments);
        await _context.SaveChangesAsync();
    }

    public async Task<LoanRequest> VerifyLoanRequest(int Id)
    {
        return await _context.LoanRequests
            .Include(x => x.Customer)
            .Include(x => x.TermInterestRate).FirstOrDefaultAsync(x => x.Id.Equals(Id)) ??
            throw new Exception($"No se encontro la solicitud de prestamo con el Id {Id}");
    }

    public async Task<List<Installment>> VerifyExistsInstallmentsByLoanId(int LoanId)
    {
        var Installments = await _context.Installments
            .Where(x => x.Status.ToLower() == "pending" && x.LoanId == LoanId)
            .OrderBy(x => x.DueDate)
            .ToListAsync() ??
            throw new Exception("No hay cuotas pendientes para este préstamo.");

        return Installments;
    }

    public async Task<List<Installment>> GetInstallmentsByLoanId(int Id, string? status)
    {
        var query = _context.Installments
            .Include(x => x.Loan)
            .ThenInclude(x => x.Customer)
            .Where(x => x.Loan.Id == Id);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status.ToLower() == status.ToLower());

        return await query.ToListAsync();
    }

    public async Task<Customer> VerifyCustomer(int Id)
    {
        return await _context.Customers.FindAsync(Id) ??
            throw new Exception($"No se encontro un cliente con el Id {Id}");
    }

    public async Task<Loan> GetLoanById(int Id)
    {
        return await _context.Loans
            .Include(x => x.Customer)
            .Include(x => x.Installments)
            .FirstOrDefaultAsync(x => x.Id == Id) ??
            throw new Exception("El prestamo solicitado no existe.");
    }
}
