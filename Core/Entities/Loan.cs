namespace Core.Entities;

public class Loan
{
    public int Id { get; set; }
    public string LoanType { get; set; } = string.Empty; // Personal, Hipotecario, Automotriz, etc
    public decimal Amount { get; set; } // Cantidad Solicitada
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal AmountPaid { get; set; }
    public string Status { get; set; } = string.Empty; // Aprobado, Recahazado


    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public int TermInterestId { get; set; }
    public TermInterestRate TermInterest { get; set; } = null!;

    public List<Payment> Payments { get; set; } = [];
}
