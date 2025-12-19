using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

public partial class CpinMaster
{
    public int Id { get; set; }

    public string CpinNumber { get; set; } = null!;

    public DateOnly? CpinDate { get; set; }

    public long? Amount { get; set; }
}
