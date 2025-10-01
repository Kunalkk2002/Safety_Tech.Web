using Microsoft.AspNetCore.Identity;
using System;

namespace Safety_Tech.Models.Models;

public partial class Incidents
{
    public int Id { get; set; }

    public string? Image { get; set; }

    public string? Label { get; set; }

    public double Confidence { get; set; }

    public double XMin { get; set; }

    public double YMin { get; set; }

    public double XMax { get; set; }

    public double YMax { get; set; }
    public bool IsApprove { get; set; }
    public string ApproveBy { get; set; } = string.Empty; // Admin user Id from AspNetUsers
    public IdentityUser? Approver { get; set; }


    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


