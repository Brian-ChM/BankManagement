using Core.DTOs;
using Core.Entities;
using Core.Request;

namespace Core.Interfaces.Services;

public interface IInstallmentService
{
    Task<string> PaidInstallments(InstallmentPaymentRequest installmentPayment);
    Task<List<InstallmentsDto>> GetInstallments(int id, string? status);
    Task<List<InstallmentsOverdueDto>> GetInstallmentsOverdue();

    List<Installment> GenerateInstallments(Loan loan);
}
