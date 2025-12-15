namespace PartitionTableFullStack.API.DTOs;

// Request DTOs
public class BillCreateRequestDto
{
    public DateOnly BillDate { get; set; }
    public short? BillMode { get; set; }
    public string? ReferenceNo { get; set; }
    public short TrMasterId { get; set; }
    public short PaymentMode { get; set; }
    public string? Demand { get; set; }
    public string? MajorHead { get; set; }
    public string? SubMajorHead { get; set; }
    public string? MinorHead { get; set; }
    public string? PlanStatus { get; set; }
    public string? SchemeHead { get; set; }
    public string? DetailHead { get; set; }
    public string? VotedCharged { get; set; }
    public long GrossAmount { get; set; }
    public long NetAmount { get; set; }
    public long BtAmount { get; set; }
    public string? SanctionNo { get; set; }
    public long? SanctionAmt { get; set; }
    public DateOnly? SanctionDate { get; set; }
    public string? SanctionBy { get; set; }
    public string? Remarks { get; set; }
    public string? DdoCode { get; set; }
    public string? TreasuryCode { get; set; }
    public short Status { get; set; } = 1; // Default Draft
    public bool IsGst { get; set; }
    public long? GstAmount { get; set; }

    // Sub-components
    public List<BtDetailDto> BtDetails { get; set; } = new();
    public List<GstDetailDto> GstDetails { get; set; } = new();
    public List<EcsDetailDto> EcsDetails { get; set; } = new();
    public List<AllotmentDetailDto> AllotmentDetails { get; set; } = new();
    public List<SubvoucherDetailDto> SubvoucherDetails { get; set; } = new();
}

public class BtDetailDto
{
    public int? BtSerial { get; set; }
    public short? BtType { get; set; }
    public long Amount { get; set; }
    public string? DdoCode { get; set; }
    public string? TreasuryCode { get; set; }
    public short? Status { get; set; }
}

public class GstDetailDto
{
    public long? CpinId { get; set; }
    public string? DdoGstn { get; set; }
    public string? DdoCode { get; set; }
    public short? TrId { get; set; }
}

public class EcsDetailDto
{
    public string? PayeeName { get; set; }
    public string? BeneficiaryId { get; set; }
    public string? PanNo { get; set; }
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? IfscCode { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankName { get; set; }
    public long Amount { get; set; }
    public bool IsGst { get; set; }
}

public class AllotmentDetailDto
{
    public long? AllotmentId { get; set; }
    public long Amount { get; set; }
    public string? DdoCode { get; set; }
    public string? TreasuryCode { get; set; }
    public long ActiveHoaId { get; set; }
    public long? AllotmentReceived { get; set; }
    public long? ProgressiveExpenses { get; set; }
}

public class SubvoucherDetailDto
{
    public string? SubvoucherNo { get; set; }
    public DateOnly? SubvoucherDate { get; set; }
    public long? SubvoucherAmount { get; set; }
    public string? Description { get; set; }
}

// Response DTOs
public class BillCreateResponseDto
{
    [System.Text.Json.Serialization.JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("bill_id")]
    public long? BillId { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("bill_no")]
    public string? BillNo { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("financial_year")]
    public short? FinancialYear { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("message")]
    public string? Message { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("error")]
    public string? Error { get; set; }
}

public class BillListItemDto
{
    public long BillId { get; set; }
    public string? BillNo { get; set; }
    public DateOnly BillDate { get; set; }
    public string? DdoCode { get; set; }
    public long? GrossAmount { get; set; }
    public long? NetAmount { get; set; }
    public short Status { get; set; }
    public string? Remarks { get; set; }
    public short FinancialYear { get; set; }
}

public class BillListResponseDto
{
    public List<BillListItemDto> Bills { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class BillDetailsResponseDto
{
    public long BillId { get; set; }
    public string? BillNo { get; set; }
    public DateOnly BillDate { get; set; }
    public short? BillMode { get; set; }
    public string? ReferenceNo { get; set; }
    public short TrMasterId { get; set; }
    public short PaymentMode { get; set; }
    public short FinancialYear { get; set; }
    public string? Demand { get; set; }
    public string? MajorHead { get; set; }
    public string? SubMajorHead { get; set; }
    public string? MinorHead { get; set; }
    public string? PlanStatus { get; set; }
    public string? SchemeHead { get; set; }
    public string? DetailHead { get; set; }
    public string? VotedCharged { get; set; }
    public long? GrossAmount { get; set; }
    public long? NetAmount { get; set; }
    public long? BtAmount { get; set; }
    public string? SanctionNo { get; set; }
    public long? SanctionAmt { get; set; }
    public DateOnly? SanctionDate { get; set; }
    public string? SanctionBy { get; set; }
    public string? Remarks { get; set; }
    public string? DdoCode { get; set; }
    public string? TreasuryCode { get; set; }
    public short Status { get; set; }
    public bool IsGst { get; set; }
    public long? GstAmount { get; set; }
    public DateTime? CreatedAt { get; set; }

    public List<BtDetailDto> BtDetails { get; set; } = new();
    public List<GstDetailDto> GstDetails { get; set; } = new();
    public List<EcsDetailDto> EcsDetails { get; set; } = new();
    public List<AllotmentDetailDto> AllotmentDetails { get; set; } = new();
    public List<SubvoucherDetailDto> SubvoucherDetails { get; set; } = new();
}

public class BillReportSummaryDto
{
    public short FinancialYear { get; set; }
    public string? FinancialYearText { get; set; }
    public int TotalBills { get; set; }
    public long TotalGrossAmount { get; set; }
    public long TotalNetAmount { get; set; }
    public long TotalBtAmount { get; set; }
    public long TotalGstAmount { get; set; }
    public List<BillReportItemDto> Bills { get; set; } = new();
}

public class BillReportItemDto
{
    public string? BillNo { get; set; }
    public DateOnly BillDate { get; set; }
    public string? DdoCode { get; set; }
    public long? GrossAmount { get; set; }
    public long? NetAmount { get; set; }
    public long? BtAmount { get; set; }
    public long? GstAmount { get; set; }
    public short Status { get; set; }
}
