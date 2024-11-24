using Core.Request;
using FluentValidation;

namespace Infrastructure.Validation;

public class SimulateLoanValidation : AbstractValidator<LoanSimulateRequest>
{
    public SimulateLoanValidation()
    {
        RuleFor(x => x.Amount).NotEmpty().GreaterThan(0);
        RuleFor(x => x.Month).NotEmpty().GreaterThan(0);
    }
}
