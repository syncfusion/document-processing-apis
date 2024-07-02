using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackgroundWorker
{
    public class BackgroundRunner : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly int _workerPollSize;
        private readonly ILogger<BackgroundRunner> _logger;
        private Timer _timer;

        public BackgroundRunner(string workerPollSize, IServiceScopeFactory serviceScopeFactory, ILogger<BackgroundRunner> logger)
        {
            if (!int.TryParse(workerPollSize, out _workerPollSize) || _workerPollSize <= 0)
            {
                throw new ArgumentException("Invalid workerPollSize value", nameof(workerPollSize));
            }

            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var deleteResource = Environment.GetEnvironmentVariable("ENABLE_DELETE_RESOURCE") ?? "true";

            if (!string.IsNullOrEmpty(deleteResource) && bool.TryParse(deleteResource, out var isDeleteResource) && isDeleteResource)
            {
                // Delete resource is disabled
                _timer = new Timer(DeleteResources, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
            }

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExecuteJobBatchAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private async Task ExecuteJobBatchAsync(CancellationToken cancellationToken)
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var jobMessageService = scope.ServiceProvider.GetRequiredService<IJobStorageService>();
            var jobExecutor = scope.ServiceProvider.GetRequiredService<IJobExecutor>();

            if (!jobMessageService.IsConnected())
            {
                Console.WriteLine("Database is not connected");
                return;
            }

            var tasks = new List<Task>();

            for (int i = 0; i < _workerPollSize; i++)
            {
                var job = await jobMessageService.DequeueJobAsync().ConfigureAwait(false);

                if (job != null)
                {
                    var status = await jobMessageService.GetJobStatus(job.ID).ConfigureAwait(false);

                    if (!string.IsNullOrEmpty(job.Status) && job.Status.ToLower() == "queued")
                    {
                        await jobMessageService.SetStatus(job.ID, "in progress").ConfigureAwait(false);

                        Task task = Task.Run(async () =>
                        {
                            _logger.LogInformation($"Processing job {job.ID}");

                            try
                            {
                                await jobExecutor.ExecuteAsync(job, scope).ConfigureAwait(false);
                                await jobMessageService.CompleteJobAsync(job).ConfigureAwait(false);
                                _logger.LogInformation($"Job {job.ID} completed");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogInformation($"Job {job.ID} failed with error: {ex.Message}");
                                jobExecutor.HandleException(job, ex);
                                await jobMessageService.CompleteJobWithErrorAsync(job).ConfigureAwait(false);
                            }

                        }, cancellationToken);
                        tasks.Add(task);
                    }
                }
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            if (_timer != null)
                _timer?.Change(Timeout.Infinite, 0);

            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            if(_timer != null)
                _timer?.Dispose();
            base.Dispose();
        }
        private async void DeleteResources(object state)
        {
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var jobMessageService = scope.ServiceProvider.GetRequiredService<IJobStorageService>();
                if (jobMessageService.IsConnected())
                {
                    var jobs = await jobMessageService.GetCompletedJobs();
                    var jobExecutor = scope.ServiceProvider.GetRequiredService<IJobExecutor>();
                    foreach (var job in jobs)
                    {
                        jobExecutor.DeleteResources(job, scope);
                        await jobMessageService.DeleteJob(job.ID);
                    }
                }
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting resources");
            }
        }
    }

}