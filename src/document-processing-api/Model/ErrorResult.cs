using System.Net;

namespace OfficeToPdf.API.Model
{

    /// <summary>
    /// Return error details in case of failure.
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// Status code of the error
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Error details
        /// </summary>
        public string Error { get; set; }

    }

}
