using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

public partial class Treasury
{
    public short Id { get; set; }

    public string TreasuryCode { get; set; } = null!;

    public string TreasuryName { get; set; } = null!;

    public bool? IsActive { get; set; }
}
