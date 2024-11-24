using Core.DTOs;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Core.Request;
using Mapster;
using FluentValidation;

namespace Infrastructure.Services;

public class BankService : IBankService
{
    private readonly IBankRepository _bankRepository;
    private readonly IValidator<LoanApplicationRequest> _loanApplicationValidation;

    public BankService(IBankRepository bankRepository, IValidator<LoanApplicationRequest> loanApplicationValidation)
    {
        _bankRepository = bankRepository;
        _loanApplicationValidation = loanApplicationValidation;
    }

    public async Task<CustomerDto> GetToken(int Id)
    {
        return await _bankRepository.GetToken(Id);
    }

    public async Task<LoanApproveDto> ApproveLoan(int LoanRequestId)
    {
        var LoanEntity = await _bankRepository.ApproveLoan(LoanRequestId);
        var Installments = GenerateInstallments(LoanEntity);
        await _bankRepository.AddInstallments(Installments);
        return LoanEntity.Adapt<LoanApproveDto>();
    }

    public async Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication)
    {
        var results = await _loanApplicationValidation.ValidateAsync(loanApplication);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);


        await _bankRepository.GetMonthsByMonths(loanApplication.MonthRequest);

        return await _bankRepository.AddLoanApplication(loanApplication);
    }

    public List<Installment> GenerateInstallments(Loan loan)
    {
        var installments = new List<Installment>();

        decimal principal = loan.Amount;
        decimal monthlyInterestRate = loan.InterestRate / 12 / 100;
        int months = loan.Months;

        decimal monthlyPayment = principal * monthlyInterestRate *
                                 (decimal)Math.Pow((double)(1 + monthlyInterestRate), months) /
                                 (decimal)(Math.Pow((double)(1 + monthlyInterestRate), months) - 1);

        for (int i = 1; i <= months; i++)
        {
            var DueDate = new DateTime(loan.AprovedDate.Year, loan.AprovedDate.Month, 1).AddMonths(i);
            decimal interestAmount = principal * monthlyInterestRate;
            decimal principalAmount = monthlyPayment - interestAmount;
            principal -= principalAmount;


            var Installment = new Installment
            {
                LoanId = loan.Id,
                TotalAmount = monthlyPayment,
                PrincipalAmount = principalAmount,
                InterestAmount = interestAmount,
                DueDate = DateTime.SpecifyKind(DueDate, DateTimeKind.Utc),
                Status = "Pending",
            };
            installments.Add(Installment);
        }

        return installments;
    }

}
