using DocumentProcessing.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal class DeletePdfServices : IDeletePdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public DeletePdfServices(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> DeletePdf(DeletePdfSettingsDTO settings)
        {
            Stream docStream = await _fileStorageService.DownloadFile(settings.File, settings.JobID);
            PdfLoadedDocument loadedDocument = string.IsNullOrEmpty(settings.Password) ? new PdfLoadedDocument(docStream) :
                           new PdfLoadedDocument(docStream, settings.Password);
            HashSet<int> deletedPages = new HashSet<int>();

            foreach (PageRangeDTO pageRange in settings.PageRanges)
            {
                int startPage = pageRange.Start;
                int endPage = pageRange.End;
                for (int i = startPage; i <= endPage && i < loadedDocument.Pages.Count; i++)
                {
                    if (!deletedPages.Contains(i))
                    {

                        deletedPages.Add(i);
                    }
                }
            }

            MemoryStream stream = new MemoryStream();
            if (deletedPages.Count > 1)
            {
                PdfDocument document = new PdfDocument();
                for (int i = 0; i < loadedDocument.Pages.Count; i++)
                {
                    if (!deletedPages.Contains(i))
                    {
                        document.ImportPage(loadedDocument, i);
                    }
                }
                document.Save(stream);
                document.Close(true);
            }
            else if(deletedPages.Count == 1)
            {
                loadedDocument.Pages.RemoveAt(deletedPages.First());
                loadedDocument.Save(stream);
            }
            else
            {
                loadedDocument.Save(stream);
                //Dispose the PDF resources.
                loadedDocument.Close(true);
            }
         
            stream.Position = 0;
            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".pdf", settings.JobID);
            stream.Dispose();
            return file;
        }
    }
}
