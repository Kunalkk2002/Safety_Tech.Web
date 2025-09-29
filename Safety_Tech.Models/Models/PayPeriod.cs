using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

public partial class PayPeriod
{
    public int PayPeriodId { get; set; }

    public int PayPeriodStartDay { get; set; }

    public int PayPeriodEndDay { get; set; }
}
