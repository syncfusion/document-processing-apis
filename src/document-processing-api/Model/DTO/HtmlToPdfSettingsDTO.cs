namespace DocumentProcessing.API.Model.DTO
{
    internal class HtmlToPdfSettingsDTO
    {
        public string? IndexFile { get; set; }

        public string? Url { get; set; }

        public List<string>? Assets { get; set; }

        public int? Margin { get; set; }

        public int? AdditionalDelay { get; set; }

        public int? ViewPortWidth { get; set; }

        public string JobID { get; set; }
    }
}
