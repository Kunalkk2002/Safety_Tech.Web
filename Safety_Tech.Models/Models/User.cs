namespace Safety_Tech.Models.Models;

/// <summary>
/// Represents a user entity in the application.
/// </summary>
public partial class User //: IdentityUser
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string? UserMail { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is active.
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets the collection of payment statuses associated with the user.
    /// </summary>
    public virtual ICollection<PaymentStatus> PaymentStatuses { get; set; } = new List<PaymentStatus>();

    /// <summary>
    /// Gets or sets the collection of payments associated with the user.
    /// </summary>
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    /// <summary>
    /// Gets or sets the collection of work time entries associated with the user.
    /// </summary>
    public virtual ICollection<WorkTimeEntry> WorkTimeEntries { get; set; } = new List<WorkTimeEntry>();

    /// <summary>
    /// Gets or sets the collection of roles associated with the user.
    /// </summary>
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    /// <summary>
    /// Gets or sets the collection of refresh tokens associated with the user.
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
