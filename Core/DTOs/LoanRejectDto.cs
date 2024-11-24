namespace Core.DTOs;

public class LoanRejectDto
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
}
