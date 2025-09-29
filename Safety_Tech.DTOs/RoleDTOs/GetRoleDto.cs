namespace Safety_Tech.DTOs.RoleDTOs
{
    /// <summary>
    /// Data Transfer Object for retrieving role information.
    /// </summary>
    public class GetRoleDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the role.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the user ID associated with the role.
        /// </summary>
        public int UserId { get; set; }
    }
}
