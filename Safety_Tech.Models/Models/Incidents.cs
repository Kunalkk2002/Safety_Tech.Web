using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Safety_Tech.Models.Models;

[Table("Incidents")]
public partial class Incidents
{
    [Key]
    public Guid Id { get; set; }

    //public string? Image { get; set; }

    //public string? Label { get; set; }

    //public double Confidence { get; set; }

    //public double XMin { get; set; }

    //public double YMin { get; set; }

    //public double XMax { get; set; }

    //public double YMax { get; set; }

    public int No { get; set; }                 
    public string? CameraName { get; set; }      
    public string? CameraId { get; set; }        
    public DateTime Timestamp { get; set; }    
    public string? TrackId { get; set; }         
    public string? MissingLabels { get; set; }  
    public string? ViolationType { get; set; } 
    public bool IsApprove { get; set; }
    public string? ApproveBy { get; set; } = string.Empty; // Admin user Id from AspNetUsers
    public IdentityUser? Approver { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


