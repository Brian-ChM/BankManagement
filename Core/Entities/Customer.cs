namespace Core.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime BirthDate { get; set; }

    public List<Loan> Loans { get; set; } = [];
    public List<LoanRequest> LoanRequests { get; set; } = [];
}
