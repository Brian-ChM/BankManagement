using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Request;
using Mapster;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Services;

public class BankService : IBankService
{
    private readonly IBankRepository _bankRepository;
    private readonly IAuthService _authService;
    private readonly IValidator<LoanApplicationRequest> _loanApplicationValidation;
    private readonly IValidator<InstallmentPaymentRequest> _installmentPayment;
    private readonly IValidator<LoanRejectRequest> _loanRejectValidation;

    public BankService(IBankRepository bankRepository,
        IValidator<LoanApplicationRequest> loanApplicationValidation,
        IValidator<LoanRejectRequest> loanRejectValidation,
        IValidator<InstallmentPaymentRequest> installmentPayment,
        IAuthService authService)
    {
        _bankRepository = bankRepository;
        _loanApplicationValidation = loanApplicationValidation;
        _loanRejectValidation = loanRejectValidation;
        _installmentPayment = installmentPayment;
        _authService = authService;
    }

    public async Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication)
    {
        var results = await _loanApplicationValidation.ValidateAsync(loanApplication);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        loanApplication.LoanType = GetLoanType(loanApplication.LoanType);
        await _bankRepository.VerifyCustomer(loanApplication.CustomerId);
        await _bankRepository.GetMonthsByMonths(loanApplication.MonthRequest);

        return await _bankRepository.AddLoanRequest(loanApplication);
    }

    public async Task<LoanApproveDto> ApproveLoan(int loanRequestId)
    {

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(loanRequestId);

        var loanRequest = await _bankRepository.VerifyLoanRequest(loanRequestId);

        if (loanRequest.Status.ToLower() != "pending")
            throw new Exception($"La solicitud de préstamo ya ha sido {(loanRequest.Status.ToLower() == "reject" ? "rechazada" : "aprobada")}.");

        loanRequest.Status = "Approve";
        var approve = loanRequest.Adapt<Loan>();
        var installments = GenerateInstallments(approve);

        return await _bankRepository.ApproveLoan(approve, installments);
    }

    public async Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject)
    {
        var results = await _loanRejectValidation.ValidateAsync(loanReject);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        var loanRequest = await _bankRepository.VerifyLoanRequest(loanReject.Id);

        if (loanRequest.Status.ToLower() != "pending")
            throw new Exception($"La solicitud de préstamo ya ha sido {(loanRequest.Status.ToLower() == "reject" ? "rechazada" : "aprobada")}.");

        loanRequest.Status = "Reject";
        loanRequest.RejectionReason = loanReject.Reason;

        return await _bankRepository.RejectLoan(loanRequest);
    }

    public async Task<LoanDetailedDto> DetailedLoan(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var loan = await _bankRepository.GetLoanById(id);
        return loan.Adapt<LoanDetailedDto>();
    }

    public async Task<string> PaidInstallments(InstallmentPaymentRequest installmentPayment)
    {
        var results = await _installmentPayment.ValidateAsync(installmentPayment);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        var installments = await _bankRepository.VerifyExistsInstallmentsByLoanId(installmentPayment.Id);

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

        await _bankRepository.PaidInstallments(installments);

        return $"Pago {installmentsPaidCount} cuotas, tiene pendiente {installmentsPendings - installmentsPaidCount}";
    }

    public async Task<List<InstallmentsDto>> GetInstallments(int id, string? status)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var installments = await _bankRepository.GetInstallmentsByLoanId(id, status);
        return installments.Adapt<List<InstallmentsDto>>();
    }

    public async Task<List<InstallmentsOverdueDto>> GetInstallmentsOverdue()
    {
        var installmentsOverdue = await _bankRepository.GetInstallmentsOverdue();
        return installmentsOverdue.Adapt<List<InstallmentsOverdueDto>>();
    }


    public List<Installment> GenerateInstallments(Loan loan)
    {
        var installments = new List<Installment>();

        decimal principal = loan.Amount;
        decimal monthlyInterestRate = loan.InterestRate / 12 / 100;
        int months = loan.Months;

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

    public async Task<string> GetToken(int id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);

        var customer = await _bankRepository.VerifyCustomer(id);
        var customerDto = customer.Adapt<CustomerDto>();

        return _authService.CreateToken(customerDto);
    }
    public string GetLoanType(string value)
    {
        var validLoanTypes = new[] { "vivienda", "automotriz", "personal" };

        foreach (var loanType in validLoanTypes)
        {
            if (String.Compare(value.ToLower(), loanType, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0)
                return loanType;
        }
        throw new Exception("Tipo de prestamo no valido");
    }

}
