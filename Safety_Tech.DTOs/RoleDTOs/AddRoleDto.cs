namespace Safety_Tech.DTOs.RoleDTOs
{
    /// <summary>
    /// Data Transfer Object for adding a new role.
    /// </summary>
    public class AddRoleDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        public string RoleName { get; set; }
    }
}
