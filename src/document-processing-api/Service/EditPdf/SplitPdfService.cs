using DocumentProcessing.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Compression.Zip;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;

namespace DocumentProcessing.API.Service.EditPdf
{
    internal class SplitPdfService : ISplitPdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public SplitPdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> SplitPdf(SplitPdfSettingsDTO settings)
        {
            if (settings.SplitOption.FileCount > 0)
            {
                return await SplitPdfByFileCount(settings);
            }
            else if (settings.SplitOption.PageCount > 0)
            {
                return await SplitPdfByPageCount(settings);
            }
            else if (settings.SplitOption.PageRanges != null && settings.SplitOption.PageRanges.Count > 0)
            {
                return await SplitPdfByPageRanges(settings);
            }

            return "";
        }

        public async Task<string> SplitPdfByPageRanges(SplitPdfSettingsDTO settings)
        {
            Stream docStream = await _fileStorageService.DownloadFile(settings.InputFile, settings.jobID);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
            int fileCount = settings.SplitOption.PageRanges.Count;
            int pageCount = loadedDocument.Pages.Count;
            ZipArchive zipArchive = new ZipArchive();
            for (int i = 0; i < fileCount; i++)
            {
                int startPage = settings.SplitOption.PageRanges[i].Start;
                int endPage = settings.SplitOption.PageRanges[i].End;
                PdfDocument splitDocument = new PdfDocument();
                for (int j = startPage; j <= endPage && j < pageCount; j++)
                {
                    splitDocument.ImportPage(loadedDocument, j);
                }

                SavePDFToZip(splitDocument, zipArchive, settings.InputFile, i);
            }

            loadedDocument.Close(true);
            MemoryStream stream = new MemoryStream();
            zipArchive.Save(stream, false);
            stream.Position = 0;
            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".zip", settings.jobID);
            zipArchive.Dispose();
            stream.Dispose();
            return file;
        }

        public async Task<string> SplitPdfByPageCount(SplitPdfSettingsDTO settings)
        {
            Stream docStream = await _fileStorageService.DownloadFile(settings.InputFile, settings.jobID);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
            int totalPages = loadedDocument.Pages.Count;
            int fileCount = (int)Math.Ceiling((double)totalPages / settings.SplitOption.PageCount);
            int currentPage = 0;
            ZipArchive zipArchive = new ZipArchive();
            for (int i = 0; i < fileCount; i++)
            {
                int pagesToExtract = Math.Min(settings.SplitOption.PageCount, totalPages - currentPage);

                PdfDocument splitDocument = new PdfDocument();
                for (int j = 0; j < pagesToExtract; j++)
                {
                    splitDocument.ImportPage(loadedDocument, currentPage);
                    currentPage++;
                }

                SavePDFToZip(splitDocument, zipArchive, settings.InputFile, i);
            }

            loadedDocument.Close(true);
            MemoryStream stream = new MemoryStream();
            zipArchive.Save(stream, false);
            stream.Position = 0;
            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".zip", settings.jobID);
            zipArchive.Dispose();
            stream.Dispose();
            return file;
        }


        public async Task<string> SplitPdfByFileCount(SplitPdfSettingsDTO settings)
        {
            Stream docStream = await _fileStorageService.DownloadFile(settings.InputFile, settings.jobID);
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(docStream);
            int totalPages = loadedDocument.Pages.Count;
            if (totalPages < settings.SplitOption.FileCount)
            {
                throw new Exception("The number of pages in the document is less than the number of files to split into.");
            }
            int? pagesPerDocument = totalPages / settings.SplitOption.FileCount;
            int? remainder = totalPages % settings.SplitOption.FileCount;
            int currentPage = 0;
            ZipArchive zipArchive = new ZipArchive();
            for (int i = 0; i < settings.SplitOption.FileCount; i++)
            {
                PdfDocument splitDocument = new PdfDocument();
                for (int j = 0; j < pagesPerDocument; j++)
                {
                    splitDocument.ImportPage(loadedDocument, currentPage);
                    currentPage++;
                }
                if (i == settings.SplitOption.FileCount - 1 && remainder > 0)
                {
                    for (int j = 0; j < remainder; j++)
                    {
                        splitDocument.ImportPage(loadedDocument, currentPage);
                        currentPage++;
                    }
                }

                SavePDFToZip(splitDocument, zipArchive, settings.InputFile, i);
            }

            loadedDocument.Close(true);
            MemoryStream stream = new MemoryStream();
            zipArchive.Save(stream, false);
            stream.Position = 0;
            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".zip", settings.jobID);
            zipArchive.Dispose();
            stream.Dispose();
            return file;
        }

        internal void SavePDFToZip(PdfDocument pdfDocument, ZipArchive zipArchive, string fileName, int index)
        {
            MemoryStream splitedDocument = new MemoryStream();
            pdfDocument.Save(splitedDocument);
            pdfDocument.Close(true);
            string filename = fileName.Substring(0, fileName.Length - 12);
            zipArchive.AddItem(filename + '[' + index + ']' + ".pdf", splitedDocument, false, Syncfusion.Compression.FileAttributes.Normal);
        }
    }
}
