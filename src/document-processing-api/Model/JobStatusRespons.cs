namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Define a public class called JobStatusResponse
    /// </summary>
    public class JobStatusResponse
    {
        /// <summary>
        /// The unique identifier for the job.
        /// </summary>
        public string? JobID { get; set; }

        /// <summary>
        /// Status of the job.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Error code of the job.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Error message of the job.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
