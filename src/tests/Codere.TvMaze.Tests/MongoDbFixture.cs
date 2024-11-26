using Testcontainers.MongoDb;

namespace Codere.TvMaze.Infrastructure.Tests;

public class MongoDbFixture : IAsyncLifetime
{
    private readonly MongoDbContainer _container = new MongoDbBuilder()
        .Build();

    public async Task InitializeAsync() =>
        await _container.StartAsync();

    public async Task DisposeAsync() =>
        await _container.StopAsync();

    public string GetConnectionString() => _container.GetConnectionString();
}