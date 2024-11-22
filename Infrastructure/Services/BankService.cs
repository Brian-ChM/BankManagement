using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Request;
using FluentValidation;
using Infrastructure.Validation;

namespace Infrastructure.Services;

internal class BankService : IBankService
{
    private readonly IBankRepository _bankRepository;
    private readonly IValidator<LoanApplicationRequest> _loanApplicationValidation;

    public BankService(IBankRepository bankRepository, IValidator<LoanApplicationRequest> loanApplicationValidation)
    {
        _bankRepository = bankRepository;
        _loanApplicationValidation = loanApplicationValidation;
    }

    public async Task<LoanApplicationRequest> LoanRequest(LoanApplicationRequest loanApplication)
    {
        var results = await _loanApplicationValidation.ValidateAsync(loanApplication);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);


        await _bankRepository.GetMonthsByMonths(loanApplication.MonthRequest);

        return await _bankRepository.AddLoanApplication(loanApplication);
    }
}
