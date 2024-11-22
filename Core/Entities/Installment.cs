namespace Core.Entities;

public class Installment
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PrincipalAmount { get; set; }
    public decimal InterestAmount { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;

    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;
    public Payment Payment{ get; set; } = null!;
}
