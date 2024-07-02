using Asp.Versioning;
using BackgroundWorker;
using BackgroundWorker.Model;
using DocumentProcessing.API.Filter;
using DocumentProcessing.API.Model;
using DocumentProcessing.API.Model.DTO;
using DocumentProcessing.API.Model.Examples;
using DocumentProcessing.API.Utility;
using Microsoft.AspNetCore.Mvc;
using OfficeToPdf.API.Service;
using Swashbuckle.AspNetCore.Filters;
using WordToPdf.API.Controllers;

namespace DocumentProcessing.API.Controllers
{
    /// <summary>
    /// Enhance PDF documents with a range of capabilities. Edit PDFs by merging, splitting, rotating, deleting pages, flattening, and compressing files.
    /// </summary>
    [Route("v{version:apiVersion}/" + RouteMapping.EditPdf)]
    [ApiVersion("1.0")]
    [ApiController]
    public class EditController : Controller
    {
        private readonly IJobStorageService _jobEnqueuer;
        private readonly IFileStorageService _fileStorageService;
        private readonly ILogger<ConvertController> _logger;

        /// <summary>
        /// Initialize the EditController
        /// </summary>
        /// <param name="jobEnqueuerService"></param>
        /// <param name="fileStorageService"></param>
        /// <param name="logger"></param>
        public EditController(IJobStorageService jobEnqueuerService,
            IFileStorageService fileStorageService,
            ILogger<ConvertController> logger)
        {
            this._jobEnqueuer = jobEnqueuerService;
            this._fileStorageService = fileStorageService;
            _logger = logger;
        }


        /// <summary>
        /// Merge the PDF documents.
        /// </summary>
        /// <remarks>Combine multiple PDF files into a singl PDF file.</remarks>
        /// <param name="settings">Settings for the merge PDF.</param>
        /// <returns>ActionResult indicating the result of the merge PDF job creation.</returns>
        /// <response code="201">Merg PDF job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>

        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.MergePdf)]
        [ProducesResponseType(typeof(JobCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(MergePdfSettings), typeof(MergePdfExamples))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> MergePDF([FromForm] MergePdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.MergePdf}");

                    if (Request.Form.Files.Count < 2)
                    {
                        return BadRequest("Atlease two files are need to merge.");
                    }
                    string folder = Guid.NewGuid().ToString();

                    MergePdfSettingsDTO mergePdfSettings = new MergePdfSettingsDTO()
                    {
                        Files = new List<FileInformationDTO>()
                    };

                    foreach (var file in Request.Form.Files)
                    {
                        string uploadedFile = await _fileStorageService.UploadFileAsync(file, folder).ConfigureAwait(false);
                        FileInformation? fileInfo = settings.Files.Where(item => item.File == file.Name).FirstOrDefault();
                        if (fileInfo != null)
                        {
                            mergePdfSettings.Files.Add(new FileInformationDTO
                            {
                                File = uploadedFile,
                                Password = fileInfo.Password
                            });
                        }
                    }
                    var message = System.Text.Json.JsonSerializer.Serialize(mergePdfSettings);
                    var outputFileName = FileUtils.GenerateUniqueFileName("merged_file.pdf");

                    Job job = new Job
                    {
                        ID = folder,
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.MergePdf,
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
                    return Created("MergePDF", response);

                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception)
            {
                _logger.LogError($"POST /{RouteMapping.EditPdf}/{RouteMapping.MergePdf}");
                return StatusCode(500, $"There was an error processing your request.");
            }
        }


        /// <summary>
        /// Split the PDF document.
        /// </summary>
        /// <remarks>Divide individual PDF files into multiple documents using various criteria, such as page range, file count, or page count.</remarks>
        /// <param name="settings">Settings for the split PDF.</param>
        /// <returns>ActionResult indicating the result of the split PDF job creation.</returns>
        /// <response code="201">Split PDF job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>

        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.SplitPdf)]
        [ProducesResponseType(typeof(JobCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(SplitPdfSettings), typeof(SplitPdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> SplitPDF([FromForm] SplitPdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.SplitPdf}");

                    if (settings.SplitOption != null &&
                       settings.SplitOption.PageCount == 0 &&
                       settings.SplitOption.FileCount == 0 &&
                       (settings.SplitOption.PageRanges == null || settings.SplitOption.PageRanges.Count == 0))
                    {
                        return BadRequest("Please provide page count or file count or page range to split the PDF.");
                    }

                    if (Request.Form.Files.Count == 0)
                    {
                        return BadRequest("Atlease one file is need to merge.");
                    }


                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    SplitPdfSettingsDTO splitPdfSettings = new SplitPdfSettingsDTO()
                    {
                        InputFile = file,
                        Password = settings.Password,
                    };

                    if (settings.SplitOption != null)
                    {
                        splitPdfSettings.SplitOption = new SplitOptionDTO()
                        {
                            PageCount = settings.SplitOption.PageCount,
                            FileCount = settings.SplitOption.FileCount
                        };
                        if (settings.SplitOption.PageRanges != null && settings.SplitOption.PageRanges.Count > 0)
                        {
                            splitPdfSettings.SplitOption.PageRanges = new List<SplitRangeDTO>();
                            foreach (var range in settings.SplitOption.PageRanges)
                            {
                                splitPdfSettings.SplitOption.PageRanges.Add(new SplitRangeDTO
                                {
                                    Start = range.Start,
                                    End = range.End
                                });
                            }
                        }
                    }

                    var message = System.Text.Json.JsonSerializer.Serialize(splitPdfSettings);
                    var outputFileName = FileUtils.GenerateUniqueFileName("split.zip");

                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.SplitPdf,
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
                    return Created("SplitPDF", response);
                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception)
            {
                _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.SplitPdf}");
                return StatusCode(500, $"There was an error processing your request.");
            }
        }


        /// <summary>
        /// Rotate the PDF pages.
        /// </summary>
        /// <remarks>Rotate pages within a PDF document by 0, 90, 180, or 270 degrees. You have the option to specify the page range for rotation.</remarks>
        /// <param name="settings">Settings for the Roate PDF pages.</param>
        /// <returns>ActionResult indicating the result of the rotate PDF job creation.</returns>
        /// <response code="201">Rotate PDF job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.RotatePages)]
        [ProducesResponseType(typeof(JobCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(RotatePdfSettings), typeof(RotatePdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> RotatePDF([FromForm] RotatePdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.RotatePages}");

                    if (Request.Form.Files.Count == 0)
                    {
                        return BadRequest("Atlease one file is need to Rotate.");
                    }

                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    RotatePdfSettingsDTO rotatePdfSettings = new RotatePdfSettingsDTO()
                    {
                        File = file,
                        Password = settings.Password,
                        RotationAngle = settings.RotationAngle
                    };

                    if (settings != null)
                    {
                        rotatePdfSettings.PageRanges = new List<PageRangeDTO>();
                        foreach (var range in settings.PageRanges)
                        {
                            rotatePdfSettings.PageRanges.Add(new PageRangeDTO
                            {
                                Start = range.Start,
                                End = range.End
                            });
                        }
                    }

                    var message = System.Text.Json.JsonSerializer.Serialize(rotatePdfSettings);
                    var outputFileName = FileUtils.GenerateUniqueFileName(Request.Form.Files[0].FileName);

                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.RotatePdf,
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
                    return Created("RotatePDF", response);
                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception)
            {
                _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.RotatePages}");
                return StatusCode(500, $"There was an error processing your request.");
            }
        }


        /// <summary>
        /// Delete the PDF pages.
        /// </summary>
        /// <remarks>Delete pages from the PDF document. You can specify the range of the pages to delete.</remarks>
        /// <param name="settings">Settings for the delete PDF page.</param>
        /// <returns>ActionResult indicating the result of the delete PDF pages job creation.</returns>
        /// <response code="201">Delete PDF pages job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.DeletePages)]
        [ProducesResponseType(typeof(JobCreationResponse),StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(DeletePdfSettings), typeof(DeletePdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> DeletePDF([FromForm] DeletePdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.DeletePages}");

                    if (Request.Form.Files.Count == 0)
                    {
                        return BadRequest("Atlease one file is need to Delete.");
                    }

                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    DeletePdfSettingsDTO deletePdfSettings = new DeletePdfSettingsDTO()
                    {
                        File = file,
                        Password = settings.Password
                    };

                    if (settings != null)
                    {
                        deletePdfSettings.PageRanges = new List<PageRangeDTO>();
                        foreach (var range in settings.PageRanges)
                        {
                            deletePdfSettings.PageRanges.Add(new PageRangeDTO
                            {
                                Start = range.Start,
                                End = range.End
                            });
                        }
                    }

                    var message = System.Text.Json.JsonSerializer.Serialize(deletePdfSettings);
                    var outputFileName = FileUtils.GenerateUniqueFileName(Request.Form.Files[0].FileName);

                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.DeletePdf,
                        OutputFileName = outputFileName,
                        CreatedTime = DateTime.UtcNow,
                        UpdatedTime = DateTime.UtcNow
                    };

                    await _jobEnqueuer.EnqueueJobAsync(job).ConfigureAwait(false);
                    _logger.LogInformation($"Job enqueued successfully {job.ID}");
                    return Created("DeletePDF", new { jobID = job.ID, status = job.Status, createdAt = job.CreatedTime });
                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception)
            {
                _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.DeletePages}");
                return StatusCode(500, $"There was an error processing your request.");
            }
        }


        /// <summary>
        /// Compress the PDF document.
        /// </summary>
        /// <remarks>Reduce the PDF file size by compressing its elements, including images, fonts, metadata, and by flattening forms and annotations.</remarks>
        /// <param name="settings">Settings for the compress PDF.</param>
        /// <returns>ActionResult indicating the result of the compress PDF job creation.</returns>
        /// <response code="201">Compress PDF job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.CompressPdf)]
        [ProducesResponseType(typeof(JobCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(CompressPdfSettings), typeof(CompressPdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> CompressPDF([FromForm] CompressPdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.CompressPdf}");

                    if (Request.Form.Files.Count < 1)
                    {
                        return BadRequest("No File found to merge.");
                    }

                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    CompressPdfSettingsDTO compressPdfSettings = new CompressPdfSettingsDTO()
                    {
                        File = file,
                        Password = settings.Password,
                        ImageQuality = settings.ImageQuality,
                        OptimizeFont = settings.OptimizeFont,
                        RemoveMetadata = settings.RemoveMetadata,
                        OptimizePageContents = settings.OptimizePageContents,
                        FlattenFormFields = settings.FlattenFormFields,
                        FlattenAnnotations = settings.FlattenAnnotations
                    };


                    var message = System.Text.Json.JsonSerializer.Serialize(compressPdfSettings);
                    var outputFileName = FileUtils.GenerateUniqueFileName("compressed_file.pdf");

                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = "requested",
                        Type = OperationType.CompressPdf,
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
                    return Created("CompressPDF", response);

                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception)
            {
                _logger.LogError($"POST /{RouteMapping.EditPdf}/{RouteMapping.CompressPdf}");
                return StatusCode(500, $"There was an error processing your request.");
            }
        }


        /// <summary>
        /// Flatten the PDF document.
        /// </summary>
        /// <remarks>Make the PDF simpler by turning interactive parts, like form fields and notes, into regular content embedded in the document.</remarks>
        /// <param name="settings">Settings for the Flatten PDF.</param>
        /// <returns>ActionResult indicating the result of the Flatten PDF job creation.</returns>
        /// <response code="201">Flatten PDF job created successfully.</response>
        /// <response code="400">Bad Request. The request was invalid or cannot be otherwise served.</response>
        /// <response code="500">Internal Server Error. The server has encountered an error and is unable to process your request at this time.</response>
        /// <response code="404">Resource Not Found.</response>
        /// <response code="401">Unauthorized.</response>
        [HttpPost]
        [JWTAuthentication]
        [Route(RouteMapping.FlattenPdf)]
        [ProducesResponseType(typeof(JobCreationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerRequestExample(typeof(FlattenPdfSettings), typeof(FlattenPdfExample))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ConversionResponseExample))]
        public async Task<IActionResult> FlattenPDF([FromForm] FlattenPdfSettings settings)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"POST /{RouteMapping.EditPdf}/{RouteMapping.FlattenPdf}");

                    if (Request.Form.Files.Count < 1)
                    {
                        return BadRequest("No file found to flatten.");
                    }

                    string file = await _fileStorageService.UploadFileAsync(Request.Form.Files[0]).ConfigureAwait(false);

                    FlattenPdfSettingsDTO flattenPdfSettings = new FlattenPdfSettingsDTO()
                    {
                        File = file,
                        Password = settings.Password,
                        FlattenFormFields = settings.FlattenFormFields,
                        FlattenAnnotations = settings.FlattenAnnotations
                    };


                    var message = System.Text.Json.JsonSerializer.Serialize(flattenPdfSettings);
                    var outputFileName = FileUtils.GenerateUniqueFileName("Flattened_file.pdf");

                    Job job = new Job
                    {
                        ID = Path.GetFileNameWithoutExtension(file),
                        Message = message,
                        Status = JobStatus.Requested,
                        Type = OperationType.FlattenPdf,
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
                    return Created("FlattenPDF", response);

                }
                else
                {
                    return BadRequest("Bad Request.");
                }
            }
            catch (Exception)
            {
                _logger.LogError($"POST /{RouteMapping.EditPdf}/{RouteMapping.FlattenPdf}");
                return StatusCode(500, $"There was an error processing your request.");
            }
        }

        /// <summary>
        /// Poll the status of the edit PDF job for completion.
        /// </summary>
        /// <param name="jobID">Job ID of the request</param>
        /// <returns>ActionResult indicating the result of the edit PDF job.</returns>
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
                _logger.LogInformation($"GET /{RouteMapping.EditPdf}/status/{jobID}");
                var status = await _jobEnqueuer.GetJobStatus(jobID).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(status))
                {
                    if (status.ToLower().Equals(JobStatus.Completed))
                    {
                        var job = await _jobEnqueuer.GetJobByID(jobID).ConfigureAwait(false);
                        var stream = await _fileStorageService.DownloadFile(job.OutputFile, job.ID);

                        string mimeType = "application/pdf";

                        if (job.Type == OperationType.SplitPdf)
                        {
                            mimeType = "application/zip";
                        }

                        return File(stream, mimeType, job.OutputFileName);
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
