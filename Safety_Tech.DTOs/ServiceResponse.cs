namespace Safety_Tech.DTOs
{
    /// <summary>
    /// Generic service response wrapper for API/service results.
    /// </summary>
    /// <typeparam name="T">Type of the data returned in the response.</typeparam>
    public class ServiceResponse<T>
    {
        /// <summary>
        /// Gets or sets the data returned by the service.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the count of items (if applicable).
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the service call was successful.
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Gets or sets a message describing the result of the service call.
        /// </summary>
        public string Message { get; set; } = null;
    }
}
