namespace UserRegistrationApp.Models
{
    /// <summary>
    /// Represents a standard API response.
    /// </summary>
    /// <typeparam name="T">The type of the data included in the response.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether the API call was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the data returned by the API call.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the message associated with the API call.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents an error response.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the API call was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the error details.
        /// </summary>
        public ErrorDetails Error { get; set; }
    }

    /// <summary>
    /// Represents the details of an error.
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        public string Code { get; set; }

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


