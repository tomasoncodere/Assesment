using Bogus;
using Codere.TvMaze.Host.Authentication;
using Codere.TvMaze.Host.Infrastructure;
using Codere.TvMaze.Infrastructure.Storage;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NSubstitute;

namespace Codere.TvMaze.Infrastructure.Tests;

[Trait(Traits.Category, Traits.Categories.Integration)]
public class MongoDbApiKeyStorageTests : MongoDbIntegrationTest, IClassFixture<MongoDbFixture>
{
    private readonly Faker _faker;
    private readonly ILogger<MongoDbApiKeyStorage> _loggerMock;

    public MongoDbApiKeyStorageTests(MongoDbFixture fixture) :base(fixture)
    {
        _faker = new Faker();
        _loggerMock = Substitute.For<ILogger<MongoDbApiKeyStorage>>();
    }

    [Fact]
    public async Task Find_ShouldReturnNull_WhenFilterDoesNotMatch()
    {
        // Arrange
        var sut = new MongoDbApiKeyStorage(new MongoClient(GetConnectionString()), _loggerMock);

        // Act
        var actualApiKey = await sut.GetActiveApiKey(_faker.Random.Hash());

        // Assert
        actualApiKey.Should().BeNull();
    }

    [Fact]
    public async Task Find_ShouldReturnValidApiKey_WhenActiveApiKeyFound()
    {
        // Arrange
        var activeKey = _faker.Random.Hash();
        var expectedApiKey = new ApiKey(activeKey, true);
        await SeedDatabase([expectedApiKey, new ApiKey(_faker.Random.Hash(), false)]);

        var sut = new MongoDbApiKeyStorage(new MongoClient(GetConnectionString()), _loggerMock);

        // Act
        var actualApiKey = await sut.GetActiveApiKey(activeKey);

        // Assert
        actualApiKey.Should().Be(expectedApiKey);
    }

    [Fact]
    public async Task Find_ShouldReturnNull_WhenNoActiveApiKeyFound()
    {
        // Arrange
        var inactiveApiKey = _faker.Random.Hash();
        await SeedDatabase([
            new ApiKey(_faker.Random.Hash(), true),
            new ApiKey(inactiveApiKey, false)]);

        var sut = new MongoDbApiKeyStorage(new MongoClient(GetConnectionString()), _loggerMock);

        // Act
        var actualApiKey = await sut.GetActiveApiKey(inactiveApiKey);

        // Assert
        actualApiKey.Should().BeNull();
    }

    private async Task SeedDatabase(ApiKey[] apiKeys) =>
        await new MongoClient(GetConnectionString())
            .GetDatabase(MongoDbNames.Database)
            .GetCollection<ApiKey>(MongoDbNames.Collections.ApiKeys)
            .InsertManyAsync(apiKeys);


}