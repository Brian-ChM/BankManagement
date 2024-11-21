using Core.DTOs;
using Core.Interfaces.Services;
using Core.Request;
using FluentValidation;
using FluentValidation.Results;

namespace Infrastructure.Services;

public class SimulateService : ISimulateService
{
    private readonly IValidator<LoanRequest> _loanValidator;

    public SimulateService(IValidator<LoanRequest> loanValidator)
    {
        _loanValidator = loanValidator;
    }

    public async Task<LoanDto> SimulateLoan(LoanRequest request)
    {

        ValidationResult result = await _loanValidator.ValidateAsync(request);

        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        decimal Interest = 18;

        decimal MonthlyInterest = Interest / 100 / 12;

        decimal MonthlyPayment = request.Amount * (MonthlyInterest * (decimal)Math.Pow((double)(1 + MonthlyInterest), request.Fees))
                                 / ((decimal)Math.Pow((double)(1 + MonthlyInterest), request.Fees) - 1);

        decimal TotalAmountPaid = MonthlyPayment * request.Fees;

        decimal InterestAmount = TotalAmountPaid - request.Amount;

        return new LoanDto
        {
            MonthlyPaid = $"{MonthlyPayment:f0}",
            TotalPaid = $"{TotalAmountPaid:f0}"
        };
    }

}
