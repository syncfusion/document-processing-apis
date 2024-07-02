using Swashbuckle.AspNetCore.Filters;
using WordToPdf.API.Model;

namespace DocumentProcessing.API.Model.Examples
{ 

    public class MergePdfExamples: IExamplesProvider<MergePdfSettings>
    {
        public MergePdfSettings GetExamples()
        {
            return new MergePdfSettings
            {
                Files = new List<FileInformation>{
                    new FileInformation { File = "invoice1.pdf", Password = "password" },
                    new FileInformation { File = "invoice2.pdf", Password = "password" }
                },
                PreserveBookmarks = true
            };
        }
    }

    public class SplitPdfExample : IMultipleExamplesProvider<SplitPdfSettings>
    {
        public IEnumerable<SwaggerExample<SplitPdfSettings>> GetExamples()
        {
            yield return SwaggerExample.Create("Split by file count", new SplitPdfSettings
            {
                File = "invoice.pdf",
                Password = "password",
                SplitOption = new SplitOption { FileCount = 2 }
            });

            yield return SwaggerExample.Create("Split by page count", new SplitPdfSettings
            {
                File = "invoice.pdf",
                Password = "password",
                SplitOption = new SplitOption { PageCount = 2 }
            });

            yield return SwaggerExample.Create("Split by page range", new SplitPdfSettings
            {
                File = "invoice.pdf",
                Password = "password",
                SplitOption = new SplitOption { 
                PageRanges = new List<SplitRange> { new SplitRange { Start = 1, End = 2 } }
                }
            });
        }
    }

    public class RotatePdfExample : IExamplesProvider<RotatePdfSettings>
    {
        public RotatePdfSettings GetExamples()
        {
            return new RotatePdfSettings
            {
                File = "invoice.pdf",
                Password = "password",
                RotationAngle = "180",
                PageRanges = new List<PageRange> { new PageRange { Start = 1, End = 2 } }
            };
        }
    }

    public class DeletePdfExample : IExamplesProvider<DeletePdfSettings>
    {
        public DeletePdfSettings GetExamples()
        {
            return new DeletePdfSettings
            {
                File = "invoice.pdf",
                Password = "password",
                PageRanges = new List<PageRange> { new PageRange { Start = 1, End = 2 } }
            };
        }
    }

    public class CompressPdfExample : IExamplesProvider<CompressPdfSettings>
    {
        public CompressPdfSettings GetExamples()
        {
            return new CompressPdfSettings
            {
                File = "invoice.pdf",
                Password = "password",
                ImageQuality = 50,
                OptimizeFont= true,
                RemoveMetadata = true,
                OptimizePageContents = true,
                FlattenFormFields = true,
                FlattenAnnotations = true
            };
        }
    }

    public class FlattenPdfExample : IExamplesProvider<FlattenPdfSettings>
    {
        public FlattenPdfSettings GetExamples()
        {
            return new FlattenPdfSettings
            {
                File = "invoice.pdf",
                Password = "password",
                FlattenAnnotations = true,
                FlattenFormFields = true
            };
        }
    }
}
