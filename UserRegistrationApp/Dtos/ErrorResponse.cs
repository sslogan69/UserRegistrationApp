namespace UserRegistrationApp.Dtos
{
    /// <summary>
    /// Represents an error response.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the details of the error.
        /// </summary>
        public string Details { get; set; }
    }
}

