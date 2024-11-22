using Core.Entities;
using Core.Request;
using Mapster;

namespace Infrastructure.Mapping;

internal class LoanRequestMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoanApplicationRequest, LoanRequest>()
            .Map(dest => dest.Amount, src => src.AmountRequest);

    }
}