namespace Core.DTOs;

public class LoanDetailedDto
{
    public CustomerDetailedDto Customer { get; set; } = new();
    public string ApproveDate { get; set; } = string.Empty;
    public decimal AmountRequest { get;set; }
    public decimal TotalPaid { get;set; }
    public decimal EarnedProfit { get;set;}
    public int Months { get;set;}
    public string LoanType { get;set;} = string.Empty;
    public decimal Interest { get;set;}
    public int DuesPaid { get;set;}
    public int PendingInstallments { get;set;}
    public string NextExpirationDate { get; set; } = string.Empty;
}