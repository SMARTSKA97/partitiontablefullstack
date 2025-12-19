using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

public partial class Ddo
{
    public string DdoCode { get; set; } = null!;

    public string DdoName { get; set; } = null!;

    public bool? IsActive { get; set; }
}
