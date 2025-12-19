using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

public partial class RbiIfscStock
{
    public int Id { get; set; }

    public string IfscCode { get; set; } = null!;

    public string? BankName { get; set; }

    public string? BranchName { get; set; }
}
