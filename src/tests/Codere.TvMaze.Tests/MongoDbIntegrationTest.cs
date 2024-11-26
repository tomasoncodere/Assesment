using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Host.Authentication;
using Codere.TvMaze.Infrastructure.Storage;
using MongoDB.Driver;

namespace Codere.TvMaze.Infrastructure.Tests;



public class MongoDbIntegrationTest : IAsyncLifetime
{
    private readonly MongoDbFixture _fixture;

    protected MongoDbIntegrationTest(MongoDbFixture fixture) => 
        _fixture = fixture;

    protected string GetConnectionString() => _fixture.GetConnectionString();

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        var mongoDb = new MongoClient(_fixture.GetConnectionString()).GetDatabase(MongoDbNames.Database);

        await mongoDb.GetCollection<TvShow>(MongoDbNames.Collections.TvShows).DeleteManyAsync(FilterDefinition<TvShow>.Empty);
        await mongoDb.GetCollection<ApiKey>(MongoDbNames.Collections.ApiKeys).DeleteManyAsync(FilterDefinition<ApiKey>.Empty);
    }
}