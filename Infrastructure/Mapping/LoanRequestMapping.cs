using Core.DTOs;
using Core.Entities;
using Core.Request;
using Mapster;

namespace Infrastructure.Mapping;

public class LoanRequestMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoanApplicationRequest, LoanRequest>()
            .Map(dest => dest.Amount, src => src.AmountRequest);

        config.NewConfig<LoanRequest, Loan>()
            .Map(dest => dest.LoanRequestId, src => src.Id)
            .Map(dest => dest.CustomerId, src => src.Customer.Id)
            .Map(dest => dest.AprovedDate, src => DateTime.UtcNow)
            .Map(dest => dest.Months, src => src.TermInterestRate.Months)
            .Map(dest => dest.InterestRate, src => src.TermInterestRate.Interest)
            .Ignore(dest => dest.LoanRequest)
            .Ignore(dest => dest.Customer);

        config.NewConfig<Loan, LoanApproveDto>()
            .Map(dest => dest.ApprovedDate, src => src.AprovedDate.ToShortDateString())
            .Map(dest => dest.Interest, src => src.InterestRate);


        config.NewConfig<Loan, LoanDetailedDto>()
            .Map(dest => dest.Customer,
                 src => new LoanCustomerDetailedDto
                 {
                     Id = src.Customer.Id,
                     Name = $"{src.Customer.FirstName} {src.Customer.LastName}"
                 })
            .Map(dest => dest.ApproveDate, src => src.AprovedDate.ToShortDateString())
            .Map(dest => dest.AmountRequest, src => src.Amount)
            .Map(dest => dest.TotalPaid, src => src.Installments.Sum(x => x.TotalAmount))
            .Map(dest => dest.EarnedProfit, src => src.Installments.Sum(x => x.TotalAmount) - src.Amount)
            .Map(dest => dest.Interest, src => src.InterestRate)
            .Map(dest => dest.DuesPaid, src => src.Installments.Count(x => x.Status == "Paid"))
            .Map(dest => dest.PendingInstallments, src => src.Installments.Count(x => x.Status == "Pending"))
            .Map(dest => dest.NextExpirationDate, src => NextExpiration(src));
    }
    private static string NextExpiration(Loan src)
    {
        return src.Installments
            .FirstOrDefault(x => x.Status == "Pending")?
            .DueDate.ToShortDateString() ??
            "No hay cuotas pendientes.";
    }
}