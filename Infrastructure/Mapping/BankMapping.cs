﻿using Core.DTOs;
using Core.Entities;
using Core.Request;
using Mapster;

namespace Infrastructure.Mapping;

public class BankMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LoanRequest, LoanRequestDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Message, src => $"Solicitud lista, estado {src.Status}");

        config.NewConfig<LoanApplicationRequest, LoanRequest>()
            .Map(dest => dest.Amount, src => src.AmountRequest)
            .Map(dest => dest.LoanType, src => src.LoanType);

        config.NewConfig<LoanRequest, Loan>()
            .Map(dest => dest.LoanRequestId, src => src.Id)
            .Map(dest => dest.CustomerId, src => src.Customer.Id)
            .Map(dest => dest.AprovedDate, src => DateTime.UtcNow)
            .Map(dest => dest.Months, src => src.TermInterestRate.Months)
            .Map(dest => dest.InterestRate, src => src.TermInterestRate.Interest)
            .Ignore(dest => dest.LoanRequest)
            .Ignore(dest => dest.Customer);

        config.NewConfig<Loan, LoanApproveDto>()
            .Map(dest => dest.LoanId, src => src.Id)
            .Map(dest => dest.ApprovedDate, src => src.AprovedDate.ToShortDateString())
            .Map(dest => dest.Interest, src => src.InterestRate);

        config.NewConfig<Installment, InstallmentsDto>()
            .Map(dest => dest.Customer,
                 src => new CustomerDetailedDto
                 {
                     Id = src.Loan.Customer.Id,
                     Name = $"{src.Loan.Customer.FirstName} {src.Loan.Customer.LastName}"
                 })
            .Map(dest => dest.TotalAmount, src => src.TotalAmount)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.DueDate, src => src.DueDate.ToShortDateString());

        config.NewConfig<Installment, InstallmentsOverdueDto>()
            .Map(dest => dest.Customer,
                 src => new CustomerDetailedDto
                 {
                     Id = src.Loan.Customer.Id,
                     Name = $"{src.Loan.Customer.FirstName} {src.Loan.Customer.LastName}"
                 })
            .Map(dest => dest.DueDate, src => src.DueDate.ToShortDateString())
            .Map(dest => dest.DaysLate, src => $"Cuenta con {Math.Max(0, (DateTime.UtcNow.Date - src.DueDate.Date).Days)} días de atraso.")
            .Map(dest => dest.AmountPending, src => src.TotalAmount);


        config.NewConfig<Loan, LoanDetailedDto>()
            .Map(dest => dest.Customer,
                 src => new CustomerDetailedDto
                 {
                     Id = src.Customer.Id,
                     Name = $"{src.Customer.FirstName} {src.Customer.LastName}"
                 })
            .Map(dest => dest.ApproveDate, src => src.AprovedDate.ToShortDateString())
            .Map(dest => dest.AmountRequest, src => src.Amount)
            .Map(dest => dest.TotalPaid, src => src.Installments.Sum(x => x.TotalAmount))
            .Map(dest => dest.EarnedProfit, src => src.Installments.Sum(x => x.TotalAmount) - src.Amount)
            .Map(dest => dest.Interest, src => src.InterestRate)
            .Map(dest => dest.DuesPaid, src => src.Installments.Where(x => x.Status.ToLower() == "paid").Count())
            .Map(dest => dest.PendingInstallments, src => src.Installments.Where(x => x.Status.ToLower() == "pending" && x.DueDate < DateTime.UtcNow.Date).Count())
            .Map(dest => dest.NextExpirationDate, src => NextExpiration(src));
    }
    private static string NextExpiration(Loan src)
    {
        return src.Installments
            .OrderBy(x => x.DueDate)
            .FirstOrDefault(x => x.DueDate >= DateTime.UtcNow.Date)?
            .DueDate.ToShortDateString() ??
            "No hay cuotas pendientes.";
    }
}