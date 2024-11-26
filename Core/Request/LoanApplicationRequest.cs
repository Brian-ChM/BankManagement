namespace Core.Request;

public class LoanApplicationRequest
{
    public int CustomerId { get; set; }
    public string LoanType { get; set; } = string.Empty;
    public int AmountRequest { get; set; }
    public int MonthRequest { get; set; }
}
