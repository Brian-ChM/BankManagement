using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class LoanRequestConfiguration : IEntityTypeConfiguration<LoanRequest>
{
    public void Configure(EntityTypeBuilder<LoanRequest> entity)
    {
        entity.HasKey(x => x.Id);

        entity
            .Property(x => x.LoanType)
            .IsRequired();

        entity
            .Property(x => x.Amount)
            .IsRequired();

        entity
            .Property(x => x.Status)
            .IsRequired();

        entity
            .Property(x => x.RejectionReason)
            .IsRequired();

        entity
            .Property(x => x.CustomerId)
            .IsRequired();

        entity
            .Property(x => x.TermInterestRateId)
            .IsRequired();

        entity
            .HasOne(x => x.Customer)
            .WithMany(x => x.LoanRequests)
            .HasForeignKey(x => x.CustomerId);

        entity
            .HasOne(x => x.TermInterestRate)
            .WithMany(x => x.LoanRequests)
            .HasForeignKey(x => x.TermInterestRateId);

        entity
            .HasOne(x => x.Loan)
            .WithOne(x => x.LoanRequest)
            .HasForeignKey<Loan>(x => x.LoanRequestId);
    }
}
