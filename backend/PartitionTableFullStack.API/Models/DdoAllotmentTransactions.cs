using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

/// <summary>
/// DDO budget allotment tracking
/// </summary>
public partial class DdoAllotmentTransactions
{
    public long Id { get; set; }

    public short FinancialYear { get; set; }

    public string? DdoCode { get; set; }

    public string? TreasuryCode { get; set; }

    public long AllotmentAmount { get; set; }

    public int? ActiveHoaId { get; set; }

    public DateOnly? TransactionDate { get; set; }

    public DateTime? CreatedAt { get; set; }
}
