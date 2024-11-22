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
            .Map(dest => dest.CustomerId, src => src.CustomerId) // Mapea CustomerId
            .Map(dest => dest.LoanRequestId, src => src.Id) // Mapea LoanRequestId
            .Map(dest => dest.AprovedDate, src => DateTime.UtcNow) // Asigna la fecha de aprobación a la fecha actual
            .Map(dest => dest.Amount, src => src.Amount) // Mapea Amount
            .Map(dest => dest.Months, src => src.TermInterestRate.Months) // Mapea Months desde TermInterestRate
            .Map(dest => dest.InterestRate, src => src.TermInterestRate.Interest) // Mapea InterestRate desde TermInterestRate
            .Map(dest => dest.LoanType, src => src.LoanType); // Mapea LoanType

    //config.NewConfig<LoanRequest, Loan>()
    //    .Map(dest => dest.LoanRequestId, src => src.Id)
    //    .Map(dest => dest.Months, src => src.TermInterestRate.Months)
    //    .Map(dest => dest.InterestRate, src => src.TermInterestRate.Interest)
    //    .Map(dest => dest.AprovedDate, src => DateTime.UtcNow);

    config.NewConfig<Loan, LoanApproveDto>()
            .Map(dest => dest.ApprovedDate, src => src.AprovedDate.ToShortDateString())
            .Map(dest => dest.Interest, src => src.InterestRate);
}
}