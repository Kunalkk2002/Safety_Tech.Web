using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safety_Tech.DTOs.incidents
{
    public class Incidents
    {
        public Guid Id { get; set; }

        public string? Image { get; set; }

        public string? Label { get; set; }

        public double Confidence { get; set; }

        public double XMin { get; set; }

        public double YMin { get; set; }

        public double XMax { get; set; }

        public double YMax { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
