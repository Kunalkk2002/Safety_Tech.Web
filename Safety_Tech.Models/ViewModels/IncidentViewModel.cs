using Safety_Tech.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Tech.Models.ViewModels
{
    public class IncidentViewModel
    {
        public List<ApprovedIncident>? ApprovedIncidents { get; set; }

        public int PotentialIncidentsCount { get; set; }
        public int ConfirmIncidentsCount { get; set; }

    }
}
