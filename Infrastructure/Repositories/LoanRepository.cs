using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Request;
using Infrastructure.Context;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _context;
    private readonly IInstallmentRepository _installmentRepository;

    public LoanRepository(AppDbContext context, IInstallmentRepository installmentRepository)
    {
        _context = context;
        _installmentRepository = installmentRepository;
    }

    public async Task<LoanRequestDto> AddLoanRequest(LoanApplicationRequest loanApplication)
    {
        var termInterest = await _installmentRepository.GetMonthsByMonths(loanApplication.MonthRequest);

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

    public async Task<LoanRequest> VerifyLoanRequest(int id)
    {
        return await _context.LoanRequests
            .Include(x => x.Customer)
            .Include(x => x.TermInterestRate).FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
            throw new Exception($"No se encontro la solicitud de prestamo con el Id {id}");
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
