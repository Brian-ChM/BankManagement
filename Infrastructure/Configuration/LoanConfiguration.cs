using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> entity)
    {
        entity.HasKey(x => x.Id);

        entity
            .Property(x => x.LoanType)
            .IsRequired();

        entity
            .Property(x => x.Amount)
            .IsRequired();

        entity
            .Property(x => x.StartDate)
            .IsRequired();

        entity
            .Property(x => x.EndDate)
            .IsRequired();

        entity
            .Property(x => x.Status)
            .IsRequired();

        entity
            .Property(x => x.AmountPaid)
            .IsRequired();

        entity
            .Property(x => x.CustomerId)
            .IsRequired();

        entity
            .HasOne(x => x.Customer)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.CustomerId);

        entity
            .Property(x => x.TermInterestId)
            .IsRequired();

        entity
            .HasOne(x => x.TermInterest)
            .WithOne(x => x.Loan)
            .HasForeignKey<Loan>(x => x.TermInterestId)
            .IsRequired();

        entity
            .HasMany(x => x.Payments)
            .WithOne(x => x.Loan)
            .HasForeignKey(x => x.LoanId);
    }
}
