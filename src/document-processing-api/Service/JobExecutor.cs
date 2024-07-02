using BackgroundWorker;
using OfficeToPdf.API.Model.DTO;
using OfficeToPdf.API.Model;
using System.Net;
using System.Text.Json;
using DocumentProcessing.API.Model.DTO;
using DocumentProcessing.API.Service.OfficeToPdf;
using DocumentProcessing.API.Service.EditPdf;
using DocumentProcessing.API.Utility;


namespace OfficeToPdf.API.Service
{
    internal class JobExecutor : IJobExecutor
    {
        public async Task ExecuteAsync(Job job, AsyncServiceScope scope)
        {
            await ExecuteOfficeService(job, scope).ConfigureAwait(false);
        }

        public void HandleException(Job job, Exception ex)
        {
            var errorResult = HandlingJobExceptions(ex);
            job.ErrorMessage = errorResult.Error;
            job.ErrorStatusCode = (int)errorResult.StatusCode;
        }


        private async Task ExecuteOfficeService(Job job, AsyncServiceScope scope)
        {
            //Code to convert word, excel, powerpoint to pdf
            switch (job.Type)
            {
                case OperationType.WordToPdf:
                    await ConvertWordToPDF(job, scope);
                    break;
                case OperationType.ExcelToPdf:
                    await ExcelToPdf(job, scope);
                    break;
                case OperationType.PowerpointToPdf:
                    await PowerpointToPdf(job, scope);
                    break;
                case OperationType.MergePdf:
                    await MergePdf(job, scope);
                    break;
                case OperationType.SplitPdf:
                    await SplitPdf(job, scope);
                    break;
                case OperationType.RotatePdf:
                    await RotatePdf(job, scope);
                    break;
                case OperationType.DeletePdf:
                    await DeletePdf(job, scope);
                    break;
                case OperationType.CompressPdf:
                    await CompressPdf(job, scope);
                    break;
                case OperationType.FlattenPdf:
                    await FlattenPdf(job, scope);
                    break;
                case OperationType.HtmlToPdf:
                    await HtmlToPdf(job, scope);
                    break;
            }
        }

        private async Task HtmlToPdf(Job job, AsyncServiceScope scope)
        {
            var htmlToPdfService = scope.ServiceProvider.GetRequiredService<IHtmlToPdfService>();
            var htmlToPdfSettings = JsonSerializer.Deserialize<HtmlToPdfSettingsDTO>(job.Message);
            if (htmlToPdfSettings == null)
            {
                throw new Exception("Html settings are not provided");
            }
            htmlToPdfSettings.JobID = job.ID;
            job.OutputFile = await htmlToPdfService.ConvertHtmlToPdf(htmlToPdfSettings).ConfigureAwait(false);
        }

        private async Task FlattenPdf(Job job, AsyncServiceScope scope)
        {
            var flattenPdfService = scope.ServiceProvider.GetRequiredService<IFlattenPdfService>();
            var flattenPdfSettings = JsonSerializer.Deserialize<FlattenPdfSettingsDTO>(job.Message);
            if (flattenPdfSettings == null)
            {
                throw new Exception("Flatten settings are not provided");
            }
            flattenPdfSettings.JobID = job.ID;
            job.OutputFile = await flattenPdfService.FlattenPdf(flattenPdfSettings).ConfigureAwait(false);
        }
        private async Task CompressPdf(Job job, AsyncServiceScope scope)
        {
            var compressPdfService = scope.ServiceProvider.GetRequiredService<ICompressPdfService>();
            var compressPdfSettings = JsonSerializer.Deserialize<CompressPdfSettingsDTO>(job.Message);
            if (compressPdfSettings == null)
            {
                throw new Exception("Compress settings are not provided");
            }
            compressPdfSettings.JobID = job.ID;
            job.OutputFile = await compressPdfService.CompressPdf(compressPdfSettings).ConfigureAwait(false);
        }

        private async Task DeletePdf(Job job, AsyncServiceScope scope)
        {
            var deletePdfService = scope.ServiceProvider.GetRequiredService<IDeletePdfService>();
            var deletePdfSettings = JsonSerializer.Deserialize<DeletePdfSettingsDTO>(job.Message);
            if (deletePdfSettings == null)
            {
                throw new Exception("Delete settings are not provided");
            }
            deletePdfSettings.JobID = job.ID;
            job.OutputFile = await deletePdfService.DeletePdf(deletePdfSettings).ConfigureAwait(false);
        }
        private async Task RotatePdf(Job job, AsyncServiceScope scope)
        {
            var rotatePdfService = scope.ServiceProvider.GetRequiredService<IRotatePdfService>();
            var rotatePdfSettings = JsonSerializer.Deserialize<RotatePdfSettingsDTO>(job.Message);
            if (rotatePdfSettings == null)
            {
                throw new Exception("Rotate settings are not provided");
            }
            rotatePdfSettings.JobID = job.ID;
            job.OutputFile = await rotatePdfService.RotatePdf(rotatePdfSettings).ConfigureAwait(false);
        }
        private async Task SplitPdf(Job job, AsyncServiceScope scope)
        {
            var _pdfSplitService = scope.ServiceProvider.GetRequiredService<ISplitPdfService>();
            var splitSettings = JsonSerializer.Deserialize<SplitPdfSettingsDTO>(job.Message);
            if (splitSettings == null)
            {
                throw new Exception("Split settings are not provided");
            }
            splitSettings.jobID = job.ID;
            job.OutputFile = await _pdfSplitService.SplitPdf(splitSettings).ConfigureAwait(false);
        }
        private async Task ConvertWordToPDF(Job job, AsyncServiceScope scope)
        {
            var _wordToPdfService = scope.ServiceProvider.GetRequiredService<IWordToPdfService>();
            WordToPdfSettingsDTO? settings = JsonSerializer.Deserialize<WordToPdfSettingsDTO>(job.Message);
            if (settings == null)
            {
                throw new Exception("Word settings are not provided");
            }
            settings.JobID = job.ID;
            job.OutputFile = await _wordToPdfService.ConvertToPDF(settings).ConfigureAwait(false);
        }

        private async Task ExcelToPdf(Job job, AsyncServiceScope scope)
        {
            var _excelToPdfService = scope.ServiceProvider.GetRequiredService<IExcelToPdfService>();
            var excelSettings = JsonSerializer.Deserialize<ExcelToPdfSettingsDTO>(job.Message);
            if (excelSettings == null)
            {
                throw new Exception("Excel settings are not provided");
            }
            excelSettings.JobID = job.ID;
            job.OutputFile = await _excelToPdfService.ConvertToPDF(excelSettings).ConfigureAwait(false);
        }

        private async Task PowerpointToPdf(Job job, AsyncServiceScope scope)
        {
            var _powerPointToPdfService = scope.ServiceProvider.GetRequiredService<IPowerpointToPdfService>();
            var powerPointSettings = JsonSerializer.Deserialize<PowerpointToPdfSettingsDTO>(job.Message);
            if (powerPointSettings == null)
            {
                throw new Exception("Powerpoint settings are not provided");
            }
            powerPointSettings.JobID = job.ID;
            job.OutputFile = await _powerPointToPdfService.ConvertToPDF(powerPointSettings).ConfigureAwait(false);
        }

        private async Task MergePdf(Job job, AsyncServiceScope scope)
        {
            var _pdfMergeService = scope.ServiceProvider.GetRequiredService<IMergePdfService>();
            var mergeSettings = JsonSerializer.Deserialize<MergePdfSettingsDTO>(job.Message);
            if (mergeSettings == null)
            {
                throw new Exception("Merge settings are not provided");
            }
            mergeSettings.JobID = job.ID;
            job.OutputFile = await _pdfMergeService.MergePdf(mergeSettings).ConfigureAwait(false);
        }

        internal ErrorResult HandlingJobExceptions(Exception exception)
        {
            if (exception.Message.Contains("The password is invalid", StringComparison.InvariantCultureIgnoreCase))
            {
                return ((new ErrorResult { StatusCode = HttpStatusCode.Unauthorized, Error = "You have passed an incorrect password" }));
            }
            else if (exception.Message.Contains("Contents of file stream is empty", StringComparison.InvariantCultureIgnoreCase))
            {
                return ((new ErrorResult { StatusCode = HttpStatusCode.BadRequest, Error = "Please provide an PDF file as a input" }));
            }
            else if (exception.Message.Contains("The path is not of a legal form", StringComparison.InvariantCultureIgnoreCase))
            {
                return ((new ErrorResult { StatusCode = HttpStatusCode.BadRequest, Error = "Please provide an Image source for a watermark" }));
            }
            else if (exception.Message.Contains("Value cannot be null", StringComparison.InvariantCultureIgnoreCase))
            {
                return ((new ErrorResult { StatusCode = HttpStatusCode.BadRequest, Error = "Please provide an proper input value" }));
            }
            else if (exception.Message.Contains("Cannot recognize current file type", StringComparison.InvariantCultureIgnoreCase))
            {
                return ((new ErrorResult { StatusCode = HttpStatusCode.BadRequest, Error = "Please provide an proper input value" }));
            }
            else if (exception.Message.Contains("Possible wrong file format or archive is corrupt", StringComparison.InvariantCultureIgnoreCase))
            {
                return ((new ErrorResult { StatusCode = HttpStatusCode.BadRequest, Error = "Please provide an proper input value" }));
            }
            else
            {
                return ((new ErrorResult { StatusCode = HttpStatusCode.InternalServerError, Error = exception.Message }));
            }

        }

        public void DeleteResources(Job job, AsyncServiceScope scope)
        {
            var storageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();

            if (!String.IsNullOrEmpty(job.ID))
            {
                storageService.DeleteFolder(job.ID);
            }

        }
    }
}
