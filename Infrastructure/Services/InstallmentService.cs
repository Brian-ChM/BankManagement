using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Request;
using FluentValidation;
using Mapster;

namespace Infrastructure.Services;

public class InstallmentService : IInstallmentService
{
    private readonly IInstallmentRepository _installmentRepository;
    private readonly IValidator<InstallmentPaymentRequest> _installmentPayment;

    public InstallmentService(IInstallmentRepository installmentRepository,
        IValidator<InstallmentPaymentRequest> installmentPayment)
    {
        _installmentRepository = installmentRepository;
        _installmentPayment = installmentPayment;
    }

    public async Task<string> PaidInstallments(InstallmentPaymentRequest installmentPayment)
    {
        var results = await _installmentPayment.ValidateAsync(installmentPayment);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        var installments = await _installmentRepository.VerifyExistsInstallmentsByLoanId(installmentPayment.Id);

        if (installments.Count == 0)
            throw new Exception("No se encontro un prestamo con el Id solicitado.");

        var installmentsPendings = installments.Where(x => x.Status.Equals("pending", StringComparison.CurrentCultureIgnoreCase)).Count();

        int remainingAmount = installmentPayment.Amount;
        int installmentsPaidCount = 0;

        if (installmentsPendings <= 0)
            throw new Exception("No posee cuotas pendientes.");

        foreach (var installment in installments)
        {
            var installmentRound = (int)Math.Ceiling(installment.TotalAmount);

            if (remainingAmount >= installment.TotalAmount)
            {
                installment.Status = "paid";
                remainingAmount -= installmentRound;
                installmentsPaidCount++;
            }
            else
            {
                throw new Exception($"Pago no completado, el monto de una cuota es {installmentRound - installmentsPaidCount}.");
            }

            if (remainingAmount == 0)
                break;
        }

        if (remainingAmount > 0)
            throw new Exception("El monto proporcionado excede el total de las cuotas pendientes.");

        await _installmentRepository.PaidInstallments(installments);

        return $"Pago {installmentsPaidCount} cuotas, tiene pendiente {installmentsPendings - installmentsPaidCount}";
    }

    public async Task<List<InstallmentsDto>> GetInstallments(int id, string? status)
    {
        if (id <= 0)
        {
            throw new Exception($"El {nameof(id)} debe ser mayor que 0.");
        }

        var installments = await _installmentRepository.GetInstallmentsByLoanId(id, status);
        return installments.Adapt<List<InstallmentsDto>>();
    }

    public async Task<List<InstallmentsOverdueDto>> GetInstallmentsOverdue()
    {
        var installmentsOverdue = await _installmentRepository.GetInstallmentsOverdue();
        return installmentsOverdue.Adapt<List<InstallmentsOverdueDto>>();
    }

    public List<Installment> GenerateInstallments(Loan loan)
    {
        var installments = new List<Installment>();

        decimal principal = loan.Amount;
        int months = loan.Months;

        decimal monthlyInterestRate = loan.InterestRate / 12 / 100;

        decimal monthlyPayment = principal * monthlyInterestRate *
                                 (decimal)Math.Pow((double)(1 + monthlyInterestRate), months) /
                                 (decimal)(Math.Pow((double)(1 + monthlyInterestRate), months) - 1);

        for (int i = 1; i <= months; i++)
        {
            var dueDate = new DateTime(loan.AprovedDate.Year, loan.AprovedDate.Month, 1).AddMonths(i);
            decimal interestAmount = principal * monthlyInterestRate;
            decimal principalAmount = monthlyPayment - interestAmount;
            principal -= principalAmount;


            var installment = new Installment
            {
                LoanId = loan.Id,
                TotalAmount = monthlyPayment,
                PrincipalAmount = principalAmount,
                InterestAmount = interestAmount,
                DueDate = DateTime.SpecifyKind(dueDate, DateTimeKind.Utc),
                Status = "pending",
            };
            installments.Add(installment);
        }

        return installments;
    }

}
