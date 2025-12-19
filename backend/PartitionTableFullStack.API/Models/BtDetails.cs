using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

public partial class BtDetails
{
    public short BtSerial { get; set; }

    public string BtTypeName { get; set; } = null!;

    public bool? IsActive { get; set; }
}
