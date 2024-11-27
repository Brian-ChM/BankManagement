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

        var termInterest = await _simulate.GetMonthsByMonths(request.Month);

        decimal interest = termInterest.Interest;

        decimal monthlyInterest = interest / 100 / 12;

        decimal monthlyPayment = request.Amount * (monthlyInterest * 
                                (decimal)Math.Pow((double)(1 + monthlyInterest), request.Month)) / 
                                 ((decimal)Math.Pow((double)(1 + monthlyInterest), request.Month) - 1);

        decimal totalAmountPaid = (monthlyPayment * request.Month);

        return new LoanDto
        {
            MonthlyPaid = (int)Math.Ceiling(monthlyPayment),
            InterestRate = interest,
            TotalPaid = (int)Math.Ceiling(totalAmountPaid)
        };
    }

}
