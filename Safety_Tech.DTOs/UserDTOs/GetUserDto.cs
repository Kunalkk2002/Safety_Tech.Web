namespace Safety_Tech.DTOs.UserDTOs
{
    /// <summary>
    /// Data Transfer Object for retrieving user information.
    /// </summary>
    public class GetUserDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public string UserId { get; set; }

        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string UserMail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the name of the user's role.
        /// </summary>
        public string RoleName { get; set; }
    }
}
