using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

public partial class WorkType
{
    public int WorkTypeId { get; set; }

    public string? WorkTypeName { get; set; }

    public string? WorkTypeSign { get; set; }

    public decimal WorkTypeHours { get; set; }

    public virtual ICollection<WorkTimeEntry> WorkTimeEntries { get; set; } = new List<WorkTimeEntry>();
}
