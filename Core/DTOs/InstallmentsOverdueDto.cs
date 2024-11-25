namespace Core.DTOs;

public class InstallmentsOverdueDto
{
    public CustomerDetailedDto Customer { get; set; } = new();
    public string DueDate { get; set; } = string.Empty;
    public string DaysLate { get; set; } = string.Empty;
    public decimal AmountPending { get; set; }
}
