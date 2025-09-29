using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

public partial class ProjectTeam
{
    public int ProjectTeamId { get; set; }

    public int UserRoleId { get; set; }

    public int ProjectUserId { get; set; }

    public int ProjectId { get; set; }

    public virtual Project Project { get; set; } = null!;
}
