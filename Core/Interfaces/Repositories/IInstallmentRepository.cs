using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IInstallmentRepository
{
    Task<List<Installment>> GetInstallmentsOverdue();
    Task<TermInterestRate> GetMonthsByMonths(int months);
    Task PaidInstallments(List<Installment> installments);
    Task<List<Installment>> VerifyExistsInstallmentsByLoanId(int loanId);
    Task<List<Installment>> GetInstallmentsByLoanId(int id, string? status);
}
