using Core.Request;
using FluentValidation;

namespace Infrastructure.Validation;

public class LoanRejectValidation : AbstractValidator<LoanRejectRequest>
{
    public LoanRejectValidation()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0);

        RuleFor(x => x.Reason).NotEmpty().MinimumLength(5);
    }
}
