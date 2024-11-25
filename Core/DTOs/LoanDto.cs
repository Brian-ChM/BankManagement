namespace Core.DTOs;

public class LoanDto
{
    public int MonthlyPaid { get; set; }
    public int TotalPaid { get; set; }
    public decimal InterestRate { get; set; }
}
