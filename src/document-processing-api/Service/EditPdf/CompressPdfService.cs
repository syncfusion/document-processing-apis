using DocumentProcessing.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal class CompressPdfService : ICompressPdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public CompressPdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> CompressPdf(CompressPdfSettingsDTO settings)
        {
            Stream docStream = await _fileStorageService.DownloadFile(settings.File, settings.JobID);
            PdfLoadedDocument loadedDocument = string.IsNullOrEmpty(settings.Password) ? new PdfLoadedDocument(docStream) :
                        new PdfLoadedDocument(docStream, settings.Password);
            if (settings.FlattenFormFields.HasValue && loadedDocument.Form != null)
                loadedDocument.Form.Flatten = settings.FlattenFormFields.Value;
            if (settings.FlattenAnnotations.HasValue)
            {
                loadedDocument.FlattenAnnotations();
            }

            loadedDocument.Compress(GetCompressSettings(settings));
            MemoryStream stream = new MemoryStream();
            loadedDocument.Save(stream);
            stream.Position = 0;
            //Dispose the PDF resources.
            loadedDocument.Close(true);

            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".pdf", settings.JobID);
            stream.Dispose();
            return file;
        }

        internal PdfCompressionOptions GetCompressSettings(CompressPdfSettingsDTO settings)
        {
            PdfCompressionOptions options = new PdfCompressionOptions();
            if (settings.ImageQuality.HasValue)
            {
                options.ImageQuality = settings.ImageQuality.Value;
                options.CompressImages = true;
            }
            if (settings.OptimizeFont.HasValue)
                options.OptimizeFont = settings.OptimizeFont.Value;
            if (settings.RemoveMetadata.HasValue)
                options.RemoveMetadata = settings.RemoveMetadata.Value;
            if (settings.OptimizePageContents.HasValue)
                options.OptimizePageContents = settings.OptimizePageContents.Value;
            return options;
        }
    }
}
