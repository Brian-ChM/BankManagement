namespace Core.Entities;

public class Payment
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string Status { get; set; } = null!;

    public int InstallmentId { get; set; }
    public Installment Installment { get; set; } = null!;
}
