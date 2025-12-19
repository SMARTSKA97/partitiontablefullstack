using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

public partial class BillStatusMaster
{
    public short StatusId { get; set; }

    public string StatusName { get; set; } = null!;

    public string? StatusDescription { get; set; }
}
