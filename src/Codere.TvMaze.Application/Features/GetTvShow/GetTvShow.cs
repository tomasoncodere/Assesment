using Codere.TvMaze.Application.Contracts;
using Codere.TvMaze.Application.Entities;
using Microsoft.Extensions.Logging;

namespace Codere.TvMaze.Application.Features.GetTvShow;

/// <summary>
/// Represents a query to retrieve information about a specific TV show by its identifier.
/// </summary>
/// <param name="TvShowId">
/// The unique identifier of the TV show to be retrieved.
/// </param>
public sealed record GetTvShowQuery(int TvShowId);

/// <summary>
/// Handles the processing of <see cref="GetTvShowQuery"/> to retrieve details of a specific TV show.
/// </summary>
public sealed class GetTvShowQueryHandler : IRequestHandler<Task<TvShow?>, GetTvShowQuery>
{
    private readonly IGetTvShowStorage _getTvShowStorage;
    private readonly ILogger<GetTvShowQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTvShowQueryHandler"/> class.
    /// </summary>
    /// <param name="tvShowStorage">
    /// An implementation of <see cref="IGetTvShowStorage"/> used to access TV show data.
    /// </param>
    /// <param name="logger">
    /// A logger instance used to log the start and end of the query processing.
    /// </param>
    public GetTvShowQueryHandler(IGetTvShowStorage tvShowStorage, ILogger<GetTvShowQueryHandler> logger)
    {
        _getTvShowStorage = tvShowStorage;
        _logger = logger;
    }

    /// <inheritdoc cref="IRequestHandler{TResult,TRequest}.Handle(TRequest)"/>>
    public async Task<TvShow?> Handle(GetTvShowQuery query)
    {
        _logger.GetTvShowStarts(query.TvShowId);
        var tvShow = await _getTvShowStorage.GetTvShow(query.TvShowId);
        _logger.GetTvShowEnds(query.TvShowId);

        return tvShow;
    }
}

internal static partial class Logs
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "GetTvShowQuery starts for id: {id}")]
    public static partial void GetTvShowStarts(this ILogger<GetTvShowQueryHandler> logger, int id);

    [LoggerMessage(Level = LogLevel.Trace, Message = "GetTvShowQuery ends for id: {id}")]
    public static partial void GetTvShowEnds(this ILogger<GetTvShowQueryHandler> logger, int id);
}