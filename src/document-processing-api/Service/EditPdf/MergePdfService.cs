using DocumentProcessing.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal class MergePdfService : IMergePdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public MergePdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> MergePdf(MergePdfSettingsDTO settings)
        {
            PdfDocument mergeDocument = new PdfDocument
            {
                EnableMemoryOptimization = true
            };

            for (int i = 0; i < settings.Files.Count; i++)
            {
                Stream docStream = await _fileStorageService.DownloadFile(settings.Files[i].File, settings.JobID);
                PdfLoadedDocument loadedDocument = string.IsNullOrEmpty(settings.Files[i].Password) ? new PdfLoadedDocument(docStream) :
                               new PdfLoadedDocument(docStream, settings.Files[i].Password);
                if (settings.PreserveBookmarks.HasValue && settings.PreserveBookmarks.Value)
                {
                    //Merge PDF files with the bookmark content
                    PdfDocumentBase.Merge(mergeDocument, loadedDocument);
                }
                else
                {
                    //Merge PDF files without the bookmark content
                    mergeDocument.ImportPageRange(loadedDocument, 0, loadedDocument.Pages.Count - 1, false);
                }
                //Close the existing PDF document 
                loadedDocument.Close(true);
            }

            MemoryStream stream = new MemoryStream();
            mergeDocument.Save(stream);
            stream.Position = 0;
            //Dispose the PDF resources.
            mergeDocument.Close(true);
            PdfDocument.ClearFontCache();

            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".pdf", settings.JobID);
            stream.Dispose();
            return file;
        }
    }
}
