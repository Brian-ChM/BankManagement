using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entity)
    {
        entity.HasKey(x => x.Id);

        entity
            .Property(x => x.FirstName)
            .IsRequired();

        entity
            .Property(x => x.LastName)
            .IsRequired();

        entity
            .Property(x => x.Email)
            .IsRequired();

        entity
            .Property(x => x.Phone)
            .IsRequired();

        entity
            .Property(x => x.BirthDate)
            .IsRequired();

        entity
            .HasMany(x => x.Loans)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId);

        entity
            .HasMany(x => x.LoanRequests)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId);
    }
}
