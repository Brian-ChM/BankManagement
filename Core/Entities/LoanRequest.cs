namespace Core.Entities;

public class LoanRequest
{
    public int Id { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    public int CustomerId { get; set; }
    public int TermInterestRateId { get; set; }
    public Customer Customer { get; set; } = null!;
    public TermInterestRate TermInterestRate { get; set; } = null!;
    public Loan Loan { get; set; } = null!;
}
