using System.Threading.Channels;
using Codere.TvMaze.Application.Features.SyncTvShows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Codere.TvMaze.Infrastructure.Jobs;

public class JobQueue : BackgroundService, IJobDispatcher
{
    private readonly Channel<IJob> _jobChannel;
    private readonly TaskCompletionSource _completionSource;
    private readonly ILogger<JobQueue> _logger;

    public JobQueue(ILogger<JobQueue> logger)
    {
        _jobChannel = Channel.CreateUnbounded<IJob>();
        _completionSource = new TaskCompletionSource();
        _logger = logger;
    }

    public bool Dispatch(IJob job) =>
        _jobChannel.Writer.TryWrite(job);

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _jobChannel.Writer.Complete();

        await _completionSource.Task;
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        try
        {
            await foreach (var job in _jobChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    _logger.JobExecutionStarts();
                    await job.Execute();
                    _logger.JobExecutionEnds();
                }
                catch (Exception e)
                {
                    _logger.FailedToExecuteJob(e);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.CancellationRequested();
        }
        finally
        {
            _completionSource.TrySetResult();
        }
    }
}

internal static partial class Logs
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Job execution starts")]
    public static partial void JobExecutionStarts(this ILogger<JobQueue> logger);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Job execution ends")]
    public static partial void JobExecutionEnds(this ILogger<JobQueue> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to execute job")]
    public static partial void FailedToExecuteJob(this ILogger<JobQueue> logger, Exception e);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Cancellation requested")]
    public static partial void CancellationRequested(this ILogger<JobQueue> logger);

}