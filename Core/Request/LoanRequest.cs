namespace Core.Request;

public class LoanRequest
{
    public string LoanType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Fees { get; set; }
}