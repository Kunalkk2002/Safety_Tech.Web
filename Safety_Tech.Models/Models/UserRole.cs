using System;
using System.Collections.Generic;

namespace Safety_Tech.Models.Models;

public partial class UserRole
{
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}
