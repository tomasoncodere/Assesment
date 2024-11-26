using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Application.Features.GetTvShow;
using Codere.TvMaze.Application.Features.SyncTvShows;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Codere.TvMaze.Infrastructure.Storage;

public sealed class MongoDbTvShowStorage : IGetTvShowStorage, ISyncTvShowsStorage
{
    private readonly IMongoCollection<TvShow> _tvShowCollection;
    private readonly ILogger<MongoDbTvShowStorage> _logger;

    public MongoDbTvShowStorage(
        IMongoClient mongoClient,
        
        ILogger<MongoDbTvShowStorage> logger)
    {
        _tvShowCollection = mongoClient
            .GetDatabase(MongoDbNames.Database)
            .GetCollection<TvShow>(MongoDbNames.Collections.TvShows);

        _logger = logger;
    }

    public async Task<TvShow?> GetTvShow(int id)
    {
        try
        {
            return await _tvShowCollection
                .Find(Builders<TvShow>.Filter.Eq(m => m.Id, id))
                .FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.FailedToGetTvShowInformation(id, e);
            throw;
        }
    }

    public async Task SynchronizeTvShows(TvShow[] tvShows)
    {
        if (tvShows.Length == 0)
        {
            _logger.NoTvShowsToSynchronize();
            return;
        }

        _logger.SynchronizationStarts(tvShows.Length);

        try
        {
            var bulkOperations = BuildBulkOperations(tvShows);
            await _tvShowCollection.BulkWriteAsync(
                bulkOperations);
        }
        catch (Exception e)
        {
            _logger.FailedToSynchronizeTvShowsInformation(e);
            throw;
        }

        _logger.SynchronizationEnds();
    }

    private static IEnumerable<WriteModel<TvShow>> BuildBulkOperations(TvShow[] tvShows) =>
        BuildUpsertOperations(tvShows)
            .Concat([BuildDeleteOperation(tvShows)]);

    private static IEnumerable<WriteModel<TvShow>> BuildUpsertOperations(IEnumerable<TvShow> tvShows) =>
        tvShows.Select(tvShow =>
            new UpdateOneModel<TvShow>(
                    Builders<TvShow>.Filter.Eq(s => s.Id, tvShow.Id),
                    Builders<TvShow>.Update.Set(s => s.Name, tvShow.Name)
                )
                { IsUpsert = true });

    private static WriteModel<TvShow> BuildDeleteOperation(TvShow[] tvShows) =>
        new DeleteManyModel<TvShow>(
            Builders<TvShow>.Filter.Nin(s => s.Id, tvShows.Select(s => s.Id).ToHashSet()));

}

internal static partial class Logs
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Synchronization of {tvShowCount} tv shows starts")]
    public static partial void SynchronizationStarts(this ILogger<MongoDbTvShowStorage> logger, int tvShowCount);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Synchronization of tv shows ends")]
    public static partial void SynchronizationEnds(this ILogger<MongoDbTvShowStorage> logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "No tv shows to synchronize")]
    public static partial void NoTvShowsToSynchronize(this ILogger<MongoDbTvShowStorage> logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to synchronize tv shows")]
    public static partial void FailedToSynchronizeTvShowsInformation(this ILogger<MongoDbTvShowStorage> logger, Exception e);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to get tv show with id {id}")]
    public static partial void FailedToGetTvShowInformation(this ILogger<MongoDbTvShowStorage> logger, int id, Exception e);
}