using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

/// <summary>
/// Represents the status of a payment in the application.
/// </summary>
public partial class PaymentStatus
{
    /// <summary>
    /// Gets or sets the unique identifier for the payment status.
    /// </summary>
    public int PaymentStatusId { get; set; }

    /// <summary>
    /// Gets or sets the associated payment's ID.
    /// </summary>
    public int PaymentId { get; set; }

    /// <summary>
    /// Gets or sets the status string (e.g., Approved, Pending, Rejected).
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the comment for the payment status.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Gets or sets the reason for rejection, if any.
    /// </summary>
    public string? RejectReason { get; set; }

    /// <summary>
    /// Gets or sets the date when the status was last updated.
    /// </summary>
    public DateTime? LastUpdatedDate { get; set; }

    /// <summary>
    /// Gets or sets the associated project ID, if any.
    /// </summary>
    public int? ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the manager's user ID, if any.
    /// </summary>
    public int? ManagerId { get; set; }

    /// <summary>
    /// Gets or sets the type of status (custom application logic).
    /// </summary>
    public int? StatusType { get; set; }

    /// <summary>
    /// Gets or sets the manager user entity.
    /// </summary>
    public virtual User? Manager { get; set; }

    /// <summary>
    /// Gets or sets the associated payment entity.
    /// </summary>
    public virtual Payment Payment { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated project entity.
    /// </summary>
    public virtual Project? Project { get; set; }
}
