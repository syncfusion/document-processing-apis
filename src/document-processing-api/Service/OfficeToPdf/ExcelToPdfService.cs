using OfficeToPdf.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;

namespace DocumentProcessing.API.Service.OfficeToPdf
{
    internal class ExcelToPdfService : IExcelToPdfService
    {

        private readonly IFileStorageService _fileStorageService;
        public ExcelToPdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        public async Task<string> ConvertToPDF(ExcelToPdfSettingsDTO settings)
        {
            //Get input file
            Stream docStream = await _fileStorageService.DownloadFile(settings.InputFile, settings.JobID);

            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;

            //Open the workbook.
            IWorkbook workbook;
            if (string.IsNullOrEmpty(settings.Password))
                workbook = application.Workbooks.Open(docStream);
            else
            {
                workbook = application.Workbooks.Open(docStream, ExcelParseOptions.Default, false, settings.Password);
            }
            docStream.Dispose();

            //Instantiation of XlsIORenderer for Excel to PDF conversion.
            XlsIORenderer render = new XlsIORenderer();

            XlsIORendererSettings excelToPdfConverterSettings= new XlsIORendererSettings();

            if (!string.IsNullOrEmpty(settings.PdfComplaince))
            {
                excelToPdfConverterSettings.PdfConformanceLevel = GetConformanceLevel(settings.PdfComplaince);
            }

            //Converts Excel document into PDF document
            PdfDocument pdfDocument = render.ConvertToPDF(workbook,excelToPdfConverterSettings);

            //Dispose the resources.
            excelEngine.Dispose();

            //Saves the PDF document to MemoryStream.
            MemoryStream stream = new MemoryStream();
            pdfDocument.Save(stream);
            stream.Position = 0;
            //Dispose the PDF resources.
            pdfDocument.Close(true);
            PdfDocument.ClearFontCache();

            //Return the PDF document.
            string file = await _fileStorageService.UploadFileAsync(stream, ".pdf", settings.JobID);
            stream.Dispose();
            return file;
        }

        private PdfConformanceLevel GetConformanceLevel(string conformanceLevel)
        {
            switch (conformanceLevel)
            {
                case "PDF/A-1B":
                    return PdfConformanceLevel.Pdf_A1B;
                case "PDF/A-2B":
                    return PdfConformanceLevel.Pdf_A2B;
                case "PDF/A-3B":
                    return PdfConformanceLevel.Pdf_A3B;
                case "PDF/A-4":
                    return PdfConformanceLevel.Pdf_A4;
                default:
                    return PdfConformanceLevel.None;
            }
        }
    }
}
