using Core.Request;
using FluentValidation;

namespace Infrastructure.Validation;

public class SimulateLoanValidation : AbstractValidator<LoanRequestDto>
{
    public SimulateLoanValidation()
    {
        RuleFor(x => x.Amount).NotEmpty().ExclusiveBetween(1000000, 100000000);
        RuleFor(x => x.Fees).NotEmpty().ExclusiveBetween(1, 120);
    }
}
