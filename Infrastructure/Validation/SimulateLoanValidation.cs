using Core.Request;
using FluentValidation;

namespace Infrastructure.Validation;

public class SimulateLoanValidation : AbstractValidator<LoanSimulateRequest>
{
    public SimulateLoanValidation()
    {
        RuleFor(x => x.Amount).NotEmpty().ExclusiveBetween(1000000, 100000000);
        RuleFor(x => x.Month).NotEmpty().ExclusiveBetween(1, 120);
    }
}
