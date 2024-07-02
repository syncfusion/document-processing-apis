

namespace BackgroundWorker
{
    public interface IJobStorageService
    {
        Task EnqueueJobAsync(Job job);
        Task<Job?> DequeueJobAsync();
        Task<string> GetJobStatus(string jobID);
        Task CompleteJobAsync(Job job);
        Task CompleteJobWithErrorAsync(Job job);
        Task<Job?> GetJobByID(string jobID);
        bool IsConnected();
        Task SetStatus(string jobID, string status);

        Task DeleteJob(string ID);
        Task<List<Job>> GetCompletedJobs();

    }
}
