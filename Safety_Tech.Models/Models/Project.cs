using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

/// <summary>
/// Represents a project entity in the application.
/// </summary>
public partial class Project
{
    /// <summary>
    /// Gets or sets the unique identifier for the project.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the project number.
    /// </summary>
    public string? ProjectNumber { get; set; }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    public string? ProjectName { get; set; }

    /// <summary>
    /// Gets or sets the description of the project.
    /// </summary>
    public string? ProjectDescription { get; set; }

    /// <summary>
    /// Gets or sets the collection of payment statuses associated with the project.
    /// </summary>
    public virtual ICollection<PaymentStatus> PaymentStatuses { get; set; } = new List<PaymentStatus>();

    /// <summary>
    /// Gets or sets the collection of project teams associated with the project.
    /// </summary>
    public virtual ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();

    /// <summary>
    /// Gets or sets the collection of work time entries associated with the project.
    /// </summary>
    public virtual ICollection<WorkTimeEntry> WorkTimeEntries { get; set; } = new List<WorkTimeEntry>();
}
