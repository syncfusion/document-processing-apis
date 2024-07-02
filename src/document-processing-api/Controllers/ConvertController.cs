
using Microsoft.AspNetCore.Mvc;
using PowerpointToPdf.API.Model;
using WordToPdf.API.Model;
using OfficeToPdf.API.Service;
using OfficeToPdf.API.Model.DTO;
using Asp.Versioning;
using BackgroundWorker;
using ExcelToPdf.API.Model;
using BackgroundWorker.Model;
using DocumentProcessing.API.Utility;
using DocumentProcessing.API.Model;
using DocumentProcessing.API.Model.DTO;
using DocumentProcessing.API.Filter;
using Swashbuckle.AspNetCore.Filters;
using DocumentProcessing.API.Model.Examples;

namespace WordToPdf.API.Controllers
{
    /// <summary>
    /// The Syncfusion document processing engine can handle various file formats as input, including Word, Excel, PowerPoint, and HTML, converting them into PDF documents.
    /// </summary>
    [Route("v{version:apiVersion}/" + RouteMapping.OfficeToPdf)]
    [ApiVersion("1.0")]
    [ApiController]
    public class ConvertController : Controller
    {
        private readonly IJobStorageService _jobEnqueuer;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<ConvertController> _logger;

        /// <summary>
        /// Initialize the controller with required services.
        /// </summary>
        /// <param name="jobEnqueuerService"></param>
        /// <param name="fileStorageService"></param>
        /// <param name="logger"></param>
        public ConvertController(IJobStorageService jobEnqueuerService,
            IFileStorageService fileStorageService,
            ILogger<ConvertController> logger)
        {
            this._jobEnqueuer = jobEnqueuerService;
            this._fileStorageService = fileStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Converts a Word document to a PDF.
        /// </summary>
        /// <remarks>Converting a Word document to a PDF is easy. It supports various formats like .doc, .docx, and .rtf. You can adjust settings, such as accessibility and archiving, to suit your needs.</remarks>
        /// <param name="settings">Settings for the conversion.</param>
        /// <returns>ActionResult indicating the result of the conversion job creation.</returns>
        /// <response code="201">Word to PDF conversion job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.WordToPdf)]
        [ProducesResponseType(typeof(JobCreationResponse),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(WordToPdfSettings), typeof(WordToPdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> ConvertToPDF([FromForm] WordToPdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.OfficeToPdf}/{RouteMapping.WordToPdf}");

                    if (Request.Form.Files.Count < 1)
                    {
                        return BadRequest("Word file is needed to create PDF");
                    }

                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    var settingsDto = new WordToPdfSettingsDTO
                    {
                        InputFile = file,
                        Password = settings.Password,
                        PreserveFormFields = settings.PreserveFormFields,
                        PdfComplaince = settings.PdfComplaince,
                        EnableAccessibility = settings.EnableAccessibility
                    };
                    var message = System.Text.Json.JsonSerializer.Serialize(settingsDto);
                    var outputFileName = FileUtils.GenerateUniqueFileName(Request.Form.Files[0].FileName);
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName);
                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.WordToPdf,
                        OutputFileName = outputFileName + ".pdf",
                        CreatedTime = DateTime.UtcNow,
                        UpdatedTime = DateTime.UtcNow
                    };

                    await _jobEnqueuer.EnqueueJobAsync(job).ConfigureAwait(false);
                    _logger.LogInformation($"Job enqueued successfully {job.ID}");
                    var response = new JobCreationResponse
                    {
                        JobID = job.ID,
                        Status = job.Status,
                        CreatedAt = job.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };
                    return Created("WordToPDF", response);

                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST /{RouteMapping.OfficeToPdf}/{RouteMapping.WordToPdf}");
                return StatusCode(500, $"There was an error processing your request. {ex.Message}");
            }
        }

        /// <summary>
        /// Converts a Excel document to a PDF.
        /// </summary>
        /// <remarks>Converting an Excel document to a PDF is straightforward. You have the flexibility to adjust settings like archiving to customize the output according to your preferences.</remarks>
        /// <param name="settings">Settings for the conversion.</param>
        /// <returns>ActionResult indicating the result of the conversion job creation.</returns>
        /// <response code="201">Excel to PDF conversion job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.ExcelToPdf)]
        [ProducesResponseType(typeof(JobCreationResponse),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(ExcelToPdfSettings), typeof(ExcelToPdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> ConvertToPDF([FromForm] ExcelToPdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.OfficeToPdf}/{RouteMapping.ExcelToPdf}");
                    if (Request.Form.Files.Count < 1)
                    {
                        return BadRequest("Word file is needed to create PDF");
                    }

                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    var settingsDto = new ExcelToPdfSettingsDTO
                    {
                        InputFile = file,
                        Password = settings.Password,
                        PdfComplaince = settings.PdfComplaince,
                    };
                    var message = System.Text.Json.JsonSerializer.Serialize(settingsDto);
                    var outputFileName = FileUtils.GenerateUniqueFileName(Request.Form.Files[0].FileName);
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName);
                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.ExcelToPdf,
                        OutputFileName = outputFileName + ".pdf",
                        CreatedTime = DateTime.UtcNow,
                        UpdatedTime = DateTime.UtcNow
                    };

                    await _jobEnqueuer.EnqueueJobAsync(job).ConfigureAwait(false);
                    _logger.LogInformation($"Job enqueued successfully {job.ID}");
                    var response = new JobCreationResponse
                    {
                        JobID = job.ID,
                        Status = job.Status,
                        CreatedAt = job.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };
                    return Created("ExcelToPDF", response);

                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception)
            {
                _logger.LogInformation($"POST /{RouteMapping.OfficeToPdf}/{RouteMapping.ExcelToPdf}");
                return StatusCode(500, $"There was an error processing your request.");
            }
        }

        /// <summary>
        /// Converts a Powerpoint to a PDF.
        /// </summary>
        /// <remarks>Converting a PowerPoint presentation to a PDF is a seamless process. You have the flexibility to fine-tune settings, such as accessibility and archiving, to tailor the output to your specific needs.</remarks>
        /// <param name="settings">Settings for the conversion.</param>
        /// <returns>ActionResult indicating the result of the conversion job creation.</returns>
        /// <response code="201">Powerpoint to PDF conversion job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.PowerpointToPdf)]
        [ProducesResponseType(typeof(JobCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(PowerpointToPdfSettings), typeof(PowerPointToPdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> ConvertToPDF([FromForm] PowerpointToPdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.OfficeToPdf}/{RouteMapping.PowerpointToPdf}");

                    if (Request.Form.Files.Count < 1)
                    {
                        return BadRequest("Power point document is needed to create PDF");
                    }

                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    var settingsDto = new PowerpointToPdfSettingsDTO
                    {
                        InputFile = file,
                        Password = settings.Password,
                        PdfComplaince = settings.PdfComplaince,
                        EnableAccessibility = settings.EnableAccessibility
                    };
                    var message = System.Text.Json.JsonSerializer.Serialize(settingsDto);
                    var outputFileName = FileUtils.GenerateUniqueFileName(Request.Form.Files[0].FileName);
                    outputFileName = Path.GetFileNameWithoutExtension(outputFileName);
                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.PowerpointToPdf,
                        OutputFileName = outputFileName + ".pdf",
                        CreatedTime = DateTime.UtcNow,
                        UpdatedTime = DateTime.UtcNow
                    };

                    await _jobEnqueuer.EnqueueJobAsync(job).ConfigureAwait(false);
                    _logger.LogInformation($"Job enqueued successfully {job.ID}");
                    var response = new JobCreationResponse
                    {
                        JobID = job.ID,
                        Status = job.Status,
                        CreatedAt = job.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };
                    return Created("PowerpointToPDF", response);

                }
                else
                {
                    return BadRequest("Input file is not provided.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during the conversion process: {ex.Message}");
            }
        }


        /// <summary>
        /// Converts a HTML to a PDF.
        /// </summary>
        /// <remarks>Converting HTML to PDF is straightforward and preserves all elements such as graphics, images, text, fonts, and layout from the original HTML document.</remarks>
        /// <param name="settings">Settings for the conversion.</param>
        /// <returns>ActionResult indicating the result of the conversion job creation.</returns>
        /// <response code="201">HTML to PDF conversion job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.HtmlToPdf)]
        [ProducesResponseType(typeof(JobCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(HtmlToPdfSettings), typeof(HtmlToPdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> ConvertToPDF([FromForm] HtmlToPdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.OfficeToPdf}/{RouteMapping.HtmlToPdf}");

                    if (Request.Form.Files.Count < 1)
                    {
                        return BadRequest("Index file is need to create PDF");
                    }
                    if (string.IsNullOrEmpty(settings.IndexFile) && string.IsNullOrEmpty(settings.Url))
                    {
                        return BadRequest("Either html file or URL needed to generate PDF document.");
                    }

                    string folder = Guid.NewGuid().ToString();

                    var settingsDto = new HtmlToPdfSettingsDTO
                    {
                        IndexFile = settings.IndexFile,
                        Url = settings.Url,
                        Margin = settings.Margin,
                        AdditionalDelay = settings.AdditionalDelay,
                        ViewPortWidth = settings.ViewPortWidth,
                        Assets = settings.Assets

                    };

                    foreach (var file in Request.Form.Files)
                    {
                        string uploadedFile = await _fileStorageService.UploadFileAsync(file, file.FileName, folder).ConfigureAwait(false);
                    }

                    var message = System.Text.Json.JsonSerializer.Serialize(settingsDto);
                    var outputFileName = FileUtils.GenerateUniqueFileName("Html_to_pdf.pdf");
                    Job job = new Job
                    {
                        ID = folder,
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.HtmlToPdf,
                        OutputFileName = outputFileName,
                        CreatedTime = DateTime.UtcNow,
                        UpdatedTime = DateTime.UtcNow
                    };

                    await _jobEnqueuer.EnqueueJobAsync(job).ConfigureAwait(false);
                    _logger.LogInformation($"Job enqueued successfully {job.ID}");
                    var response = new JobCreationResponse
                    {
                        JobID = job.ID,
                        Status = job.Status,
                        CreatedAt = job.CreatedTime.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    };
                    return Created("HtmlToPDF", response);

                }
                else
                {
                    return BadRequest("Input file is not provided.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during the conversion process: {ex.Message}");
            }
        }
 
        /// <summary>
        /// Poll the status of the conversion job for completion.
        /// </summary>
        /// <param name="jobID">Job ID of the request</param>
        /// <returns>ActionResult indicating the result of the conversion job.</returns>
        /// <response code="200">A response with job completion information</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpGet]
        [JWTAuthentication]
        [Route("status/{jobID}")]
        [ProducesResponseType(typeof(JobStatusResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(StatusResponseExample))]
        public async Task<IActionResult> GetJobStatus(string jobID)
        {
            try
            {
                _logger.LogInformation($"GET /office/status/{jobID}");
                var status = await _jobEnqueuer.GetJobStatus(jobID).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.ToLower().Equals(JobStatus.Completed))
                    {
                        var job = await _jobEnqueuer.GetJobByID(jobID).ConfigureAwait(false);
                        var stream = await _fileStorageService.DownloadFile(job.OutputFile, job.ID);
                        return File(stream, "application/pdf", job.OutputFileName);
                    }
                    else if (status.ToLower().Equals(JobStatus.Error))
                    {
                        var job = await _jobEnqueuer.GetJobByID(jobID).ConfigureAwait(false);
                        var response = new JobStatusResponse
                        {
                            JobID = jobID,
                            Status = JobStatus.Error,
                            ErrorCode = job.ErrorStatusCode,
                            ErrorMessage = job.ErrorMessage
                        };
                        return Ok(response);
                    }
                    else
                    {
                        var response = new JobStatusResponse
                        {
                            JobID = jobID,
                            Status = status,
                        };
                        return Ok(response);
                    }
                }
                else
                {
                    return NotFound("The requested task is not found or deleted.");
                }
            }
            catch (Exception)
            {
                return NotFound("The requested task is not found or deleted.");
            }
        }

    }

}
