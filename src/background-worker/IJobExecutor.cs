using Microsoft.Extensions.DependencyInjection;

namespace BackgroundWorker
{
    public interface IJobExecutor
    {
        public Task ExecuteAsync(Job job, AsyncServiceScope scope);

        public void HandleException(Job job, Exception ex);

        public void DeleteResources(Job job, AsyncServiceScope scope);

    }
}
