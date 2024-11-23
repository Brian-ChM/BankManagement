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
    }
}