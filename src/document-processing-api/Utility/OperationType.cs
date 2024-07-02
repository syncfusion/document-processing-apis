namespace DocumentProcessing.API.Utility
{
    internal static class OperationType
    {
        //Convert to PDF
        public const string PowerpointToPdf = "PowerPointToPdf";
        public const string WordToPdf = "WordToPdf";
        public const string ExcelToPdf = "ExcelToPdf";
        public const string HtmlToPdf = "HtmlToPdf";

        //Edit PDF document
        public const string MergePdf = "MergePdf";
        public const string SplitPdf = "SplitPdf";
        public const string RotatePdf = "RotatePdf";
        public const string DeletePdf = "DeletePdf";
        public const string CompressPdf = "CompressPdf";
        public const string FlattenPdf = "FlattenPdf";
        
    }
}
