using DocumentProcessing.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal class FlattenPdfservice : IFlattenPdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public FlattenPdfservice(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> FlattenPdf(FlattenPdfSettingsDTO settings)
        {
            Stream docStream = await _fileStorageService.DownloadFile(settings.File, settings.JobID);
            PdfLoadedDocument loadedDocument = string.IsNullOrEmpty(settings.Password) ? new PdfLoadedDocument(docStream) :
                           new PdfLoadedDocument(docStream, settings.Password);
            if (settings.FlattenFormFields.HasValue)
            {
                if(loadedDocument.Form != null)
                {
                    loadedDocument.Form.Flatten = settings.FlattenFormFields.Value;
                }
            }
            if (settings.FlattenAnnotations.HasValue)
            {
                if (settings.FlattenAnnotations.Value)
                {
                    loadedDocument.FlattenAnnotations();
                }
            }

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
    }
}
