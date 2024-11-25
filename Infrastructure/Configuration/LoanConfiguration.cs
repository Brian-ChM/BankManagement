﻿using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> entity)
    {
        entity.HasKey(x => x.Id);

        entity
            .Property(x => x.AprovedDate)
            .IsRequired();

        entity
            .Property(x => x.Amount)
            .IsRequired();

        entity
            .Property(x => x.InterestRate)
            .IsRequired();



        entity
            .Property(x => x.LoanType)
            .IsRequired();

        entity
            .Property(x => x.CustomerId)
            .IsRequired();

        entity
            .Property(x => x.LoanRequestId)
            .IsRequired();

        entity
            .HasOne(x => x.Customer)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.CustomerId);

        entity
            .HasOne(x => x.LoanRequest)
            .WithOne(x => x.Loan)
            .HasForeignKey<Loan>(x => x.LoanRequestId)
            .IsRequired();

        entity
            .HasMany(x => x.Installments)
            .WithOne(x => x.Loan)
            .HasForeignKey(x => x.LoanId);
    }
}
