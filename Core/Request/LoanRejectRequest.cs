namespace Core.Request;

public class LoanRejectRequest
{
    public int Id { get; set; }
    public string Reason { get; set; } = string.Empty;
}
