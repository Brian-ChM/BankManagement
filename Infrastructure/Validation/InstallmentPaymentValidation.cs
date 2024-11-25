using Core.Request;
using FluentValidation;

namespace Infrastructure.Validation;

public class InstallmentPaymentValidation : AbstractValidator<InstallmentPaymentRequest>
{
    public InstallmentPaymentValidation()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.Amount).GreaterThan(0);
    }
}
