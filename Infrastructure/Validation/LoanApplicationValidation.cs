using Core.Request;
using FluentValidation;
using System.Globalization;

namespace Infrastructure.Validation;

internal class LoanApplicationValidation : AbstractValidator<LoanApplicationRequest>
{
    public LoanApplicationValidation()
    {
        RuleFor(x => x.LoanType).NotEmpty().Must(x => ValidateLoanType(x)).WithMessage("Seleccione un tipo de prestamo de valido.");
    }

    private bool ValidateLoanType(string value)
    {
        var validLoanTypes = new[] { "vivienda", "automotriz", "personal" };

        foreach (var loanType in validLoanTypes)
        {
            if (loanType.Equals(value.ToLower()))
                return true;
        }

        return false;
    }

}
