using System;

namespace Safety_Tech.Models.Models;

public partial class Objectdetection
{
    public int Id { get; set; }

    public string? Image { get; set; }

    public string? Label { get; set; }

    public double Confidence { get; set; }

    public double XMin { get; set; }

    public double YMin { get; set; }

    public double XMax { get; set; }

    public double YMax { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


