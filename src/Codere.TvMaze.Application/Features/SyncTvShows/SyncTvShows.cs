using Codere.TvMaze.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Codere.TvMaze.Application.Features.SyncTvShows;

/// <summary>
/// Represents a command to initiate the synchronization of TV shows.
/// </summary>
public sealed record SyncTvShowsCommand();

/// <summary>
/// Handles the execution of the <see cref="SyncTvShowsCommand"/> to initiate the synchronization of TV shows.
/// </summary>
public sealed class SyncTvShowsCommandHandler : IRequestHandler<bool, SyncTvShowsCommand>
{
    private readonly IJobDispatcher _jobDispatcher;
    private readonly IJob _job;
    private readonly ILogger<SyncTvShowsCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncTvShowsCommandHandler"/> class.
    /// </summary>
    /// <param name="jobDispatcher">
    /// An implementation of <see cref="IJobDispatcher"/> used to dispatch the synchronization job.
    /// </param>
    /// <param name="job">
    /// An implementation of <see cref="IJob"/> representing the synchronization task.
    /// </param>
    /// <param name="logger">
    /// A logger instance to record the start and end of the synchronization process.
    /// </param>
    public SyncTvShowsCommandHandler(IJobDispatcher jobDispatcher, IJob job, ILogger<SyncTvShowsCommandHandler> logger)
    {
        _jobDispatcher = jobDispatcher;
        _job = job;
        _logger = logger;
    }

    /// <inheritdoc cref="IRequestHandler{TResult,TRequest}.Handle(TRequest)"/>>
    public bool Handle(SyncTvShowsCommand request)
    {
        _logger.SyncTvShowsStarts();
        var dispatched = _jobDispatcher.Dispatch(_job);
        _logger.SyncTvShowsEnd();

        return dispatched;
    }
}

internal static partial class Logs
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "SyncTvShowsCommand starts")]
    public static partial void SyncTvShowsStarts(this ILogger<SyncTvShowsCommandHandler> logger);

    [LoggerMessage(Level = LogLevel.Trace, Message = "SyncTvShowsCommand ends")]
    public static partial void SyncTvShowsEnd(this ILogger<SyncTvShowsCommandHandler> logger);
}