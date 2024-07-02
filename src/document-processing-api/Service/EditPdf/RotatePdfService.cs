using DocumentProcessing.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace DocumentProcessing.API.Service.EditPdf
{
    /// <summary>
    /// Rotate PDF documents
    /// </summary>
    internal class RotatePdfService : IRotatePdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public RotatePdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> RotatePdf(RotatePdfSettingsDTO settings)
        {
            Stream docStream = await _fileStorageService.DownloadFile(settings.File, settings.JobID);
            PdfLoadedDocument loadedDocument = string.IsNullOrEmpty(settings.Password) ? new PdfLoadedDocument(docStream) :
                           new PdfLoadedDocument(docStream, settings.Password);
            foreach (PageRangeDTO pageRange in settings.PageRanges)
            {
                int startPage = pageRange.Start;
                int endPage = pageRange.End;
                for (int i = startPage; i <= endPage && i < loadedDocument.Pages.Count; i++)
                {
                    PdfPageBase page = loadedDocument.Pages[i];
                    PdfPageRotateAngle angle = GetRotateAngle(settings.RotationAngle);
                    page.Rotation = angle;
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

        private PdfPageRotateAngle GetRotateAngle(string rotationAngle)
        {
            switch (rotationAngle)
            {
                case "90":
                    return PdfPageRotateAngle.RotateAngle90;
                case "180":
                    return PdfPageRotateAngle.RotateAngle180;
                case "270":
                    return PdfPageRotateAngle.RotateAngle270;
                default:
                    return PdfPageRotateAngle.RotateAngle0;
            }
        }
    }
}
