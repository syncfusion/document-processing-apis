using ExcelToPdf.API.Model;
using PowerpointToPdf.API.Model;
using Swashbuckle.AspNetCore.Filters;
using WordToPdf.API.Model;

namespace DocumentProcessing.API.Model.Examples
{
    public class WordToPdfExample : IExamplesProvider<WordToPdfSettings>
    {
        public WordToPdfSettings GetExamples()
        {
           return new WordToPdfSettings
           {
               File = "invoice.docx",
               Password = "password",
               PreserveFormFields = true,
               PdfComplaince = "PDF/A-1B",
               EnableAccessibility = true
           };
        }
    }

    public class ExcelToPdfExample : IExamplesProvider<ExcelToPdfSettings>
    {
        public ExcelToPdfSettings GetExamples()
        {
            return new ExcelToPdfSettings
            {
                File = "invoice.xlsx",
                Password = "password",
                PdfComplaince = "PDF/A-1B"
            };
        }
    }

    public class PowerPointToPdfExample : IExamplesProvider<PowerpointToPdfSettings>
    {
        public PowerpointToPdfSettings GetExamples()
        {
            return new PowerpointToPdfSettings
            {
                File = "presentation.pptx",
                Password = "password",
                PdfComplaince = "PDF/A-1B",
                EnableAccessibility = true
            };
        }
    }

    public class HtmlToPdfExample : IExamplesProvider<HtmlToPdfSettings>
    {
        public HtmlToPdfSettings GetExamples()
        {
            return new HtmlToPdfSettings
            {
                IndexFile = "invoice.html",
                Assets = new List<string> { "style.css", "logo.png","font.ttf" },
                Margin = 10,
                AdditionalDelay = 3000,
                ViewPortWidth = 1920
            };
        }
    }

    public class StatusResponseExample:IMultipleExamplesProvider<JobStatusResponse>
    {
        public IEnumerable<SwaggerExample<JobStatusResponse>> GetExamples()
        {
            yield return SwaggerExample.Create("requested", new JobStatusResponse
            {
                Status = "requested",
                JobID = "ef0766ab-bc74-456c-8143-782e730a89df"
            });

            yield return SwaggerExample.Create("in progress", new JobStatusResponse
            {
                Status = "in progress",
                JobID = "ef0766ab-bc74-456c-8143-782e730a89df"
            });

            yield return SwaggerExample.Create("queued", new JobStatusResponse
            {
                Status = "queued",
                JobID = "ef0766ab-bc74-456c-8143-782e730a89df"
            });


            yield return SwaggerExample.Create("error", new JobStatusResponse
            {
                Status = "error",
                JobID = "ef0766ab-bc74-456c-8143-782e730a89df",
                ErrorCode = 500,
                ErrorMessage = "An error occurred while processing the document"
            });
        }
    }

    public class ConversionResponseExample : IExamplesProvider<JobCreationResponse>
    {
        public JobCreationResponse GetExamples()
        {
            return new JobCreationResponse
            {
                JobID = "ef0766ab-bc74-456c-8143-782e730a89df",
                Status = "requested",
                CreatedAt = "2021-09-01T12:00:00Z"
            };
        }
    }
}
