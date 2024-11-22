using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class AppDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Installment> Installments { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanRequest> LoanRequests { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<TermInterestRate> TermInterestRates { get; set; }

    public AppDbContext() { }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
