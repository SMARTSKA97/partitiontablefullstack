namespace PartitionTableFullStack.API.DTOs;

// Minimal DTO for list views - only essential fields
public class FinancialYearDto
{
    public short Id { get; set; }
    public string FinancialYear { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
