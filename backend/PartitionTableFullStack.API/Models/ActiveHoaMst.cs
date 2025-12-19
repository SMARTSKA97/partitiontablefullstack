using System;
using System.Collections.Generic;

namespace PartitionTableFullStack.API.Models;

public partial class ActiveHoaMst
{
    public int Id { get; set; }

    public string? Demand { get; set; }

    public string? MajorHead { get; set; }

    public string? SubMajorHead { get; set; }

    public string? MinorHead { get; set; }

    public string? PlanStatus { get; set; }

    public string? SchemeHead { get; set; }

    public string? DetailHead { get; set; }

    public string? VotedCharged { get; set; }

    public string? HoaDescription { get; set; }

    public bool? IsActive { get; set; }
}
