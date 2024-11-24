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
    private readonly IAuthService _authService;
    private readonly IValidator<LoanApplicationRequest> _loanApplicationValidation;
    private readonly IValidator<LoanRejectRequest> _loanRejectValidation;

    public BankService(IBankRepository bankRepository,
        IValidator<LoanApplicationRequest> loanApplicationValidation,
        IValidator<LoanRejectRequest> loanRejectValidation,
        IAuthService authService)
    {
        _bankRepository = bankRepository;
        _loanApplicationValidation = loanApplicationValidation;
        _loanRejectValidation = loanRejectValidation;
        _authService = authService;
    }

    public async Task<string> GetToken(int Id)
    {
        if (Id <= 0)
            throw new ArgumentOutOfRangeException(nameof(Id));

        var Customer = await _bankRepository.VerifyCustomer(Id);
        var CustomerDto = Customer.Adapt<CustomerDto>();

        return _authService.CreateToken(CustomerDto);
    }

    public async Task<LoanApproveDto> ApproveLoan(int loanRequestId)
    {
        if (loanRequestId <= 0)
            throw new ArgumentOutOfRangeException(nameof(loanRequestId));

        var LoanRequest = await _bankRepository.VerifyLoanRequest(loanRequestId);

        if (LoanRequest.Status == "Approve")
            throw new Exception("La solicitud de prestamo ya ha sido Aprovada.");

        LoanRequest.Status = "Approve";
        var Approve = LoanRequest.Adapt<Loan>();
        var Installments = GenerateInstallments(Approve);

        return await _bankRepository.ApproveLoan(Approve, Installments);
    }

    public async Task<LoanRejectDto> RejectLoan(LoanRejectRequest loanReject)
    {
        var results = await _loanRejectValidation.ValidateAsync(loanReject);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        var LoanRequest = await _bankRepository.VerifyLoanRequest(loanReject.Id);

        if (LoanRequest.Status == "Reject")
            throw new Exception("La solicitud de prestamo ya ha sido rechazada.");

        LoanRequest.Status = "Reject";
        LoanRequest.RejectionReason = loanReject.Reason;

        return await _bankRepository.RejectLoan(LoanRequest);
    }

    public async Task<LoanRequestDto> LoanRequest(LoanApplicationRequest loanApplication)
    {
        var results = await _loanApplicationValidation.ValidateAsync(loanApplication);

        if (!results.IsValid)
            throw new ValidationException(results.Errors);

        await _bankRepository.VerifyCustomer(loanApplication.CustomerId);
        await _bankRepository.GetMonthsByMonths(loanApplication.MonthRequest);

        return await _bankRepository.AddLoanApplication(loanApplication);
    }

    public async Task<LoanDetailedDto> DetailedLoan(int Id)
    {
        if (Id <= 0)
            throw new ArgumentOutOfRangeException(nameof(Id));

        var Loan = await _bankRepository.GetLoanById(Id);

        //var TotalPaid = Loan.Installments
        //    .Where(x => x.LoanId == Id)
        //    .Sum(x => x.TotalAmount);

        //var DuesPaid = Loan.Installments.Count(x => x.Status == "Paid");
        //var PendingInstallments = Loan.Installments.Count(x => x.Status == "Pending");

        //var NextExpiration = Loan.Installments
        //    .FirstOrDefault(x => x.Status == "Pending")?
        //    .DueDate.ToShortDateString();

        //var LoanCustomer = new LoanCustomerDetailedDto
        //{
        //    Id = Loan.Customer.Id,
        //    Name = $"{Loan.Customer.FirstName} {Loan.Customer.LastName}"
        //};
        //return new LoanDetailedDto
        //{
        //    Customer = LoanCustomer,
        //    ApproveDate = Loan.AprovedDate.ToShortDateString(),
        //    AmountRequest = Loan.Amount,
        //    TotalPaid = TotalPaid,
        //    EarnedProfit = TotalPaid - Loan.Amount,
        //    Months = Loan.Months,
        //    LoanType = Loan.LoanType,
        //    Interest = Loan.InterestRate,
        //    DuesPaid = DuesPaid,
        //    PendingInstallments = PendingInstallments,
        //    NextExpirationDate = NextExpiration ?? "No hay cuotas pendientes."
        //};

        return Loan.Adapt<LoanDetailedDto>();
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
