using System.ComponentModel.DataAnnotations;

namespace UserRegistrationApp.Dtos
{
    /// <summary>
    /// Data transfer object for user information.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        [Required(ErrorMessage = "User name is required")]
        [StringLength(100, ErrorMessage = "User name cannot be longer than 100 characters")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password hash.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, ErrorMessage = "Password cannot be longer than 256 characters")]
        public string PasswordHash { get; set; }
    }
}




