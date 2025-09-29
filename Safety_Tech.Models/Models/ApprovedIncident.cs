using Microsoft.AspNetCore.Identity;
using System;

namespace Safety_Tech.Models.Models
{
    public class ApprovedIncident
    {
        public int Id { get; set; }

        public int IncidentId { get; set; }

        public string ApproveBy { get; set; } = string.Empty; // Admin user Id from AspNetUsers

        public bool IsApprove { get; set; }

        public Objectdetection? Incident { get; set; }

        public IdentityUser? Approver { get; set; }
    }
}


