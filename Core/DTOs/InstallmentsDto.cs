namespace Core.DTOs;

public class InstallmentsDto
{
    public CustomerDetailedDto Customer { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public string DueDate { get; set; } = string.Empty;
    public string Status { get; set;} = string.Empty;
}
