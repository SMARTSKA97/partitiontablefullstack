using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

/// <summary>
/// Financial years (e.g., 2024-2025)
/// </summary>
public partial class FinancialYearMaster
{
    public short Id { get; set; }

    public string FinancialYear { get; set; } = null!;

    /// <summary>
    /// Short code like 2425 for partition naming
    /// </summary>
    public string FyCode { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
