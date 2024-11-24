using Core.Request;
using FluentValidation;
using System.Globalization;

namespace Infrastructure.Validation;

public class LoanApplicationValidation : AbstractValidator<LoanApplicationRequest>
{
    public LoanApplicationValidation()
    {
        RuleFor(x => x.CustomerId).NotEmpty().GreaterThan(0);

        RuleFor(x => x.LoanType).NotEmpty().Must(x => ValidateLoanType(x)).WithMessage("Seleccione un tipo de prestamo de valido.");
        
        RuleFor(x => x.AmountRequest).NotEmpty().GreaterThan(0);

        RuleFor(x => x.MonthRequest).NotEmpty().GreaterThan(0);
    }

    private bool ValidateLoanType(string value)
    {
        var validLoanTypes = new[] { "vivienda", "automotriz", "personal" };

        foreach (var loanType in validLoanTypes)
        {
            if (String.Compare(value.ToLower(), loanType, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0)
                return true;
        }

        return false;
    }

}
