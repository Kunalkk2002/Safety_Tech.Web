using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

/// <summary>
/// Represents a work time entry for a user on a project.
/// </summary>
public partial class WorkTimeEntry
{
    /// <summary>
    /// Gets or sets the unique identifier for the work time entry.
    /// </summary>
    public int WorkTimeEntryId { get; set; }

    /// <summary>
    /// Gets or sets the number of working hours recorded.
    /// </summary>
    public decimal WorkingHours { get; set; }

    /// <summary>
    /// Gets or sets the date of the work performed.
    /// </summary>
    public DateTime WorkDate { get; set; }

    /// <summary>
    /// Gets or sets the date the entry was created.
    /// </summary>
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// Gets or sets the associated project ID.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the associated work type ID.
    /// </summary>
    public int WorkTypeId { get; set; }

    /// <summary>
    /// Gets or sets the associated user ID, if any.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Gets or sets the associated project entity.
    /// </summary>
    public virtual Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated user entity, if any.
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Gets or sets the associated work type entity.
    /// </summary>
    public virtual WorkType WorkType { get; set; } = null!;
}
