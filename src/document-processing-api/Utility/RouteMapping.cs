namespace DocumentProcessing.API.Utility
{
    internal static class RouteMapping
    {
        //Convert office document to PDF
        public const string OfficeToPdf = "conversion";
        public const string WordToPdf = "word-to-pdf";
        public const string ExcelToPdf = "excel-to-pdf";
        public const string PowerpointToPdf = "powerpoint-to-pdf";
        public const string HtmlToPdf = "html-to-pdf";

        //Edit PDF document
        public const string EditPdf = "edit-pdf";
        public const string MergePdf = "merge";
        public const string SplitPdf = "split";
        public const string RotatePages = "rotate-pages";
        public const string DeletePages = "delete-pages";
        public const string CompressPdf = "compress";
        public const string FlattenPdf = "flatten";
    }
}
