using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Request;
using Mapster;
using FluentValidation;
using System.Globalization;

namespace Infrastructure.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly IInstallmentRepository _installmentRepository;
    private readonly IInstallmentService _installmentService;
    private readonly IAuthService _authService;
    private readonly IValidator<LoanApplicationRequest> _loanApplicationValidation;
    private readonly IValidator<LoanRejectRequest> _loanRejectValidation;

    public LoanService(ILoanRepository loanRepository,
        IInstallmentRepository installmentRepository,
        IInstallmentService installmentService,
        IValidator<LoanApplicationRequest> loanApplicationValidation,
        IValidator<LoanRejectRequest> loanRejectValidation,
        IAuthService authService)
    {
        _loanRepository = loanRepository;
        _installmentRepository = installmentRepository;
        _installmentService = installmentService;
        _loanApplicationValidation = loanApplicationValidation;
        _loanRejectValidation = loanRejectValidation;
        _authService = authService;
    }

    public async Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication)
    {
        var results = await _loanApplicationValidation.ValidateAsync(loanApplication);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        loanApplication.LoanType = GetLoanType(loanApplication.LoanType);
        await _loanRepository.VerifyCustomer(loanApplication.CustomerId);
        await _installmentRepository.GetMonthsByMonths(loanApplication.MonthRequest);

        return await _loanRepository.AddLoanRequest(loanApplication);
    }

    public async Task<LoanApproveDto> ApproveLoan(int loanRequestId)
    {
        if (loanRequestId <= 0)
        {
            throw new Exception($"El {nameof(loanRequestId)} debe ser mayor que 0.");
        }

        var loanRequest = await _loanRepository.VerifyLoanRequest(loanRequestId);

        if (loanRequest.Status.ToLower() != "pending")
            throw new Exception($"La solicitud de préstamo ya ha sido {(loanRequest.Status.ToLower() == "reject" ? "rechazada" : "aprobada")}.");

        loanRequest.Status = "Approve";
        var approve = loanRequest.Adapt<Loan>();
        var installments = _installmentService.GenerateInstallments(approve);

        return await _loanRepository.ApproveLoan(approve, installments);
    }

    public async Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject)
    {
        var results = await _loanRejectValidation.ValidateAsync(loanReject);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        var loanRequest = await _loanRepository.VerifyLoanRequest(loanReject.Id);

        if (loanRequest.Status.ToLower() != "pending")
            throw new Exception($"La solicitud de préstamo ya ha sido {(loanRequest.Status.ToLower() == "reject" ? "rechazada" : "aprobada")}.");

        loanRequest.Status = "Reject";
        loanRequest.RejectionReason = loanReject.Reason;

        return await _loanRepository.RejectLoan(loanRequest);
    }

    public async Task<LoanDetailedDto> DetailedLoan(int id)
    {
        if (id <= 0)
        {
            throw new Exception($"El {nameof(id)} debe ser mayor que 0.");
        }

        var loan = await _loanRepository.GetLoanById(id);
        return loan.Adapt<LoanDetailedDto>();
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
