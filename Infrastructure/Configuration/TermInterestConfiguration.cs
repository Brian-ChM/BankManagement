using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class TermInterestConfiguration : IEntityTypeConfiguration<TermInterestRate>
{
    public void Configure(EntityTypeBuilder<TermInterestRate> entity)
    {
        entity.HasKey(x => x.Id);

        entity
            .Property(x => x.Months)
            .IsRequired();

        entity
            .Property(x => x.Interest)
            .IsRequired();

        entity
            .HasMany(x => x.LoanRequests)
            .WithOne(x => x.TermInterestRate)
            .HasForeignKey(x => x.TermInterestRateId);
    }
}
