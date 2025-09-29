namespace Safety_Tech.DTOs.UserDTOs
{
    /// <summary>
    /// Data Transfer Object for user registration.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Gets or sets the username for registration.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password for registration.
        /// </summary>
        public string Password { get; set; }
    }
}
