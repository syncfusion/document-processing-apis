using Microsoft.AspNetCore.Mvc;

namespace DocumentProcessing.API.Controllers
{
    /// <summary>
    /// Index controller to disaply a welcome message.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)] 
    [ApiController]
    [Route("/")]
   
    public class IndexController : Controller
    {
        /// <summary>
        /// Index action to display a welcome message.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            string htmlContent = "<html><head><title>Welcome</title></head><body><h3>Welcome to the Syncfusion Document Processing APIs!</h3><p>Create PDFs from Word, Excel, PowerPoint, and HTML files, and edit the PDF documents.</p></body></html>";
            return Content(htmlContent, "text/html");
        }
    }
}
