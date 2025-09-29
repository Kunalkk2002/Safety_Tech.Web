using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<PaymentStatus> PaymentStatuses { get; set; } = new List<PaymentStatus>();

    public virtual User User { get; set; } = null!;
}
