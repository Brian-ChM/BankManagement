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

        var LoanRequest = await _bankRepository.VerifyLoanRequest(loanRequestId);

        if (LoanRequest.Status.ToLower() != "pending")
            throw new Exception($"La solicitud de préstamo ya ha sido {(LoanRequest.Status.ToLower() == "reject" ? "rechazada" : "aprobada")}.");

        LoanRequest.Status = "Approve";
        var Approve = LoanRequest.Adapt<Loan>();
        var Installments = GenerateInstallments(Approve);

        return await _bankRepository.ApproveLoan(Approve, Installments);
    }

    public async Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject)
    {
        var results = await _loanRejectValidation.ValidateAsync(loanReject);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        var LoanRequest = await _bankRepository.VerifyLoanRequest(loanReject.Id);

        if (LoanRequest.Status.ToLower() != "pending")
            throw new Exception($"La solicitud de préstamo ya ha sido {(LoanRequest.Status.ToLower() == "reject" ? "rechazada" : "aprobada")}.");

        LoanRequest.Status = "Reject";
        LoanRequest.RejectionReason = loanReject.Reason;

        return await _bankRepository.RejectLoan(LoanRequest);
    }

    public async Task<LoanDetailedDto> DetailedLoan(int Id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Id);

        var Loan = await _bankRepository.GetLoanById(Id);
        return Loan.Adapt<LoanDetailedDto>();
    }

    public async Task<string> PaidInstallments(InstallmentPaymentRequest installmentPayment)
    {
        var results = await _installmentPayment.ValidateAsync(installmentPayment);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        var Installments = await _bankRepository.VerifyExistsInstallmentsByLoanId(installmentPayment.Id);
        var InstallmentsPendings = Installments.Where(x => x.Status.Equals("pending", StringComparison.CurrentCultureIgnoreCase)).Count();

        int RemainingAmount = installmentPayment.Amount;
        int InstallmentsPaidCount = 0;

        if (InstallmentsPendings <= 0)
            throw new Exception("No posee cuotas pendientes.");

        foreach (var Installment in Installments)
        {
            var InstallmentRound = (int)Math.Ceiling(Installment.TotalAmount);

            if (RemainingAmount >= Installment.TotalAmount)
            {
                Installment.Status = "paid";
                RemainingAmount -= InstallmentRound;
                InstallmentsPaidCount++;
            }
            else
            {
                throw new Exception($"Pago no completado, el monto de una cuota es {InstallmentRound - InstallmentsPaidCount}.");
            }

            if (RemainingAmount == 0)
                break;
        }

        if (RemainingAmount > 0)
            throw new Exception("El monto proporcionado excede el total de las cuotas pendientes.");

        await _bankRepository.PaidInstallments(Installments);

        return $"Pago {InstallmentsPaidCount} cuotas, tiene pendiente {InstallmentsPendings - InstallmentsPaidCount}";
    }

    public async Task<List<InstallmentsDto>> GetInstallments(int Id, string? status)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Id);

        var Installments = await _bankRepository.GetInstallmentsByLoanId(Id, status);
        return Installments.Adapt<List<InstallmentsDto>>();
    }

    public async Task<List<InstallmentsOverdueDto>> GetInstallmentsOverdue()
    {
        var InstallmentsOverdue = await _bankRepository.GetInstallmentsOverdue();
        return InstallmentsOverdue.Adapt<List<InstallmentsOverdueDto>>();
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
            var DueDate = new DateTime(loan.AprovedDate.Year, loan.AprovedDate.Month, 1).AddMonths(i);
            decimal interestAmount = principal * monthlyInterestRate;
            decimal principalAmount = monthlyPayment - interestAmount;
            principal -= principalAmount;


            var Installment = new Installment
            {
                LoanId = loan.Id,
                TotalAmount = monthlyPayment,
                PrincipalAmount = principalAmount,
                InterestAmount = interestAmount,
                DueDate = DateTime.SpecifyKind(DueDate, DateTimeKind.Utc),
                Status = "pending",
            };
            installments.Add(Installment);
        }

        return installments;
    }

    public async Task<string> GetToken(int Id)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(Id);

        var Customer = await _bankRepository.VerifyCustomer(Id);
        var CustomerDto = Customer.Adapt<CustomerDto>();

        return _authService.CreateToken(CustomerDto);
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
