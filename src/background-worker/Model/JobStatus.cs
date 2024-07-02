namespace BackgroundWorker.Model
{
    public static class JobStatus
    {
        public const string Requested = "requested";
        public const string queued = "queued";
        public const string InProgress = "in progress";
        public const string Completed = "completed";
        public const string Cancelled = "cancelled";
        public const string Error = "error";
        public const string Deleted = "deleted";
    }
}
