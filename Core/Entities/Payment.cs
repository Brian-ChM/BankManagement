namespace Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public decimal Balance { get; set; }

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;
}
