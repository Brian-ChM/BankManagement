namespace Core.DTOs;

public class LoanApproveDto
{
    public int LoanId { get; set; }
    public int CustomerId { get; set; }
    public string ApprovedDate { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Months { get; set; }
    public decimal Interest { get; set; }
    public string LoanType { get; set; } = string.Empty;
}
