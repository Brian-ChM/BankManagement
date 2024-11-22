using Core.DTOs;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Request;
using FluentValidation;
using FluentValidation.Results;

namespace Infrastructure.Services;

public class SimulateService : ISimulateService
{
    private readonly IValidator<LoanSimulateRequest> _loanValidator;
    private readonly ISimulateRepository _simulate;

    public SimulateService(IValidator<LoanSimulateRequest> loanValidator, ISimulateRepository simulate)
    {
        _loanValidator = loanValidator;
        _simulate = simulate;
    }

    public async Task<LoanDto> SimulateLoan(LoanSimulateRequest request)
    {

        ValidationResult result = await _loanValidator.ValidateAsync(request);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        var TermInterest = await _simulate.GetMonthsByMonths(request.Month);

        decimal Interest = TermInterest.Interest;

        decimal MonthlyInterest = Interest / 100 / 12;

        decimal MonthlyPayment = request.Amount * (MonthlyInterest * (decimal)Math.Pow((double)(1 + MonthlyInterest), request.Month))
                                 / ((decimal)Math.Pow((double)(1 + MonthlyInterest), request.Month) - 1);

        decimal TotalAmountPaid = (MonthlyPayment * request.Month);

        decimal InterestAmount = TotalAmountPaid - request.Amount;

        return new LoanDto
        {
            MonthlyPaid = $"{MonthlyPayment.ToString("#,0")}",
            InterestRate = Interest,
            TotalPaid = $"{TotalAmountPaid.ToString("#,0")}"
        };
    }

}
