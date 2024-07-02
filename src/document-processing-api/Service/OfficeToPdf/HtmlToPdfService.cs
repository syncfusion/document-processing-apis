using DocumentProcessing.API.Model.DTO;
using OfficeToPdf.API.Service;
using Syncfusion.Drawing;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace DocumentProcessing.API.Service.OfficeToPdf
{
    /// <summary>
    /// Service to convert HTML to PDF
    /// </summary>
    internal class HtmlToPdfService : IHtmlToPdfService
    {
        private readonly IFileStorageService _fileStorageService;
        public HtmlToPdfService(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Convert HTML to PDF
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public async Task<string> ConvertHtmlToPdf(HtmlToPdfSettingsDTO settings)
        {
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.Blink);
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            blinkConverterSettings.CommandLineArguments.Add("--no-sandbox");
            blinkConverterSettings.CommandLineArguments.Add("--disable-setuid-sandbox");
       
            if (settings.ViewPortWidth.HasValue)
            {
                blinkConverterSettings.ViewPortSize = new Size(settings.ViewPortWidth.Value, 0);
            }
            else
            {
                blinkConverterSettings.ViewPortSize = new Size(1280, 0);
            }

            blinkConverterSettings.AdditionalDelay = settings.AdditionalDelay ?? 6000;
            blinkConverterSettings.EnableLazyLoadImages = true;
            blinkConverterSettings.EnableJavaScript = true;
            blinkConverterSettings.EnableHyperLink = true;
            if (settings.Margin.HasValue)
            {
                PdfMargins margin = new PdfMargins();
                margin.All = settings.Margin.Value;
                blinkConverterSettings.Margin = margin;
            }

            htmlConverter.ConverterSettings = blinkConverterSettings;


            PdfDocument document;
            if (!string.IsNullOrEmpty(settings.Url))
            {
                document = htmlConverter.Convert(settings.Url);
            }
            else
            {
                Stream docStream = await _fileStorageService.DownloadFile(settings.IndexFile, settings.JobID);
                string rootPath = _fileStorageService.GetRootFolder() + settings.JobID;
                string htmlString = new StreamReader(docStream).ReadToEnd();
                rootPath = Path.GetFullPath(rootPath);
                document = htmlConverter.Convert(htmlString, rootPath);
            }
            MemoryStream stream = new MemoryStream();
            document.Save(stream);
            stream.Position = 0;
            document.Close(true);

            string file = await _fileStorageService.UploadFileAsync(stream, ".pdf", settings.JobID);
            stream.Dispose();
            return file;
        }
    }
}
