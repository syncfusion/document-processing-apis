namespace BackgroundWorker
{
    public class Job
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Message { get; set; }
        public string OutputFileName { get; set; }
        public string OutputFile { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        public int ErrorStatusCode { get; set; }
    }
}
