namespace Safety_Tech.DTOs.UserDTOs
{
    /// <summary>
    /// Data Transfer Object for user login credentials.
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Gets or sets the username for login.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for login.
        /// </summary>
        public string Password { get; set; }
    }
}
