namespace Core.Entities;

public class Loan
{
    public int Id { get; set; }
    public DateTime AprovedDate { get; set; }
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public string LoanType { get; set; } = string.Empty;

    public int CustomerId { get; set; }
    public int LoanRequestId { get; set; }
    public Customer Customer { get; set; } = null!;
    public LoanRequest LoanRequest { get; set; } = null!;
    public List<Installment> Installments { get; set; } = [];
}
