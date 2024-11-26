using Codere.TvMaze.Host.Authentication;
using Codere.TvMaze.Host.Contracts;
using Codere.TvMaze.Infrastructure.Storage;
using MongoDB.Driver;

namespace Codere.TvMaze.Host.Infrastructure;

/// <summary>
/// Provides an implementation of <see cref="IApiKeyStorage"/> that retrieves API keys from a MongoDB collection.
/// </summary>
public class MongoDbApiKeyStorage : IApiKeyStorage
{
    private readonly IMongoCollection<ApiKey> _apiKeyCollection;
    private readonly ILogger<MongoDbApiKeyStorage> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbApiKeyStorage"/> class.
    /// </summary>
    /// <param name="mongoClient">
    /// The <see cref="IMongoClient"/> used to access the MongoDB database.
    /// </param>
    /// <param name="logger">
    /// An <see cref="ILogger"/> instance for logging operations and errors.
    /// </param>
    public MongoDbApiKeyStorage(IMongoClient mongoClient, ILogger<MongoDbApiKeyStorage> logger)
    {
        _apiKeyCollection = mongoClient
            .GetDatabase(MongoDbNames.Database)
            .GetCollection<ApiKey>(MongoDbNames.Collections.ApiKeys);
        _logger = logger;
    }

    /// <inheritdoc cref="IApiKeyStorage.GetActiveApiKey(string)"/>>
    public async Task<ApiKey?> GetActiveApiKey(string apiKeyId)
    {
        try
        {
            var filter = Builders<ApiKey>.Filter.Eq(a => a.Id, apiKeyId) &
                         Builders<ApiKey>.Filter.Eq(a => a.IsActive, true);
            return await _apiKeyCollection.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.FailedToRetrieveAuthenticationKey(e);
            throw;
        }
    }

}

internal static partial class Logs
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to retrieve authentication key")]
    public static partial void FailedToRetrieveAuthenticationKey(this ILogger<MongoDbApiKeyStorage> logger, Exception e);
}