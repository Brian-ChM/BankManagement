namespace Core.Entities;

public class TermInterestRate
{
    public int Id { get; set; }
    public ushort Months { get; set; }
    public decimal Interest {  get; set; }

    public List<LoanRequest> LoanRequests { get; set; } = [];
}
