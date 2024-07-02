namespace DocumentProcessing.API.Model
{
    /// <summary>
    /// Represents the response returned after a job creation request.
    /// </summary>
    public class JobCreationResponse
    {
        /// <summary>
        /// The unique identifier for the job.
        /// </summary>
        public string? JobID { get; set; }

        /// <summary>
        /// The status of the job.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// The timestamp when the job was created.
        /// </summary>
        public string? CreatedAt { get; set; }
    }
}
