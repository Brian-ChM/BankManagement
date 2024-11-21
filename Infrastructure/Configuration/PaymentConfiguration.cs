using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> entity)
    {
        entity.HasKey(x => x.Id);

        entity
            .Property(x => x.Amount)
            .IsRequired();

        entity
            .Property(x => x.PaymentDate)
            .IsRequired();

        entity
            .Property(x => x.Balance)
            .IsRequired();

        entity
            .Property(x => x.LoanId)
            .IsRequired();

        entity
            .HasOne(x => x.Loan)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.LoanId);
    }
}
