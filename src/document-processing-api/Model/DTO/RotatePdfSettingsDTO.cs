

namespace DocumentProcessing.API.Model.DTO
{
    internal class RotatePdfSettingsDTO
    {
        public string RotationAngle { get; set; }
        public string File { get; set; }
        public string Password { get; set; }
        public List<PageRangeDTO> PageRanges { get; set; }
        public string JobID { get; set; }

    }

    internal class PageRangeDTO
    {
        public int Start { get; set; }
        public int End { get; set; }
    }

}

