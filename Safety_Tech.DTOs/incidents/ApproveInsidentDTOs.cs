using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Tech.DTOs.incidents
{
    public class ApproveInsidentDTOs
    {
        public int Id { get; set; }

        public int IncidentId { get; set; }

        public string ApproveBy { get; set; } = string.Empty; // Admin user Id from AspNetUsers

        public bool IsApprove { get; set; }

       
    }
}
