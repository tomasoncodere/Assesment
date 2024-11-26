using Bogus;
using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Infrastructure.Storage;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NSubstitute;

namespace Codere.TvMaze.Infrastructure.Tests;

[Trait(Traits.Category, Traits.Categories.Integration)]
public class MongoDbTvShowStorageTests : MongoDbIntegrationTest, IClassFixture<MongoDbFixture>
{
    private readonly Faker _faker;
    private readonly ILogger<MongoDbTvShowStorage> _loggerMock;

    public MongoDbTvShowStorageTests(MongoDbFixture fixture) : base(fixture)
    {
        _faker = new Faker();
        _loggerMock = Substitute.For<ILogger<MongoDbTvShowStorage>>();
    }

    [Fact]
    public async Task Find_ShouldStoreAllTvShows_WhenCollectionEmpty()
    {
        // Arrange
        var expectedTvShows = new Faker<TvShow>().GenerateTvShows(3);

        var sut = new MongoDbTvShowStorage(new MongoClient(GetConnectionString()), _loggerMock);

        // Act
        await sut.SynchronizeTvShows(expectedTvShows);

        // Assert
        var actualTvShows = await new MongoClient(GetConnectionString())
            .GetDatabase(MongoDbNames.Database)
            .GetCollection<TvShow>(MongoDbNames.Collections.TvShows)
            .Find(FilterDefinition<TvShow>.Empty)
            .ToListAsync();

        actualTvShows.Should().BeEquivalentTo(expectedTvShows);
    }

    [Fact]
    public async Task Find_ShouldModifyTvShow_WhenTvShowExists()
    {
        // Arrange
        var expectedTvShows = new Faker<TvShow>().GenerateTvShows(3);

        var sut = new MongoDbTvShowStorage(new MongoClient(GetConnectionString()), _loggerMock);
        await sut.SynchronizeTvShows(expectedTvShows);

        expectedTvShows[0] = expectedTvShows[0] with { Name = _faker.Lorem.Sentence() };

        // Act
        await sut.SynchronizeTvShows(expectedTvShows);

        // Assert
        var actualTvShows = await new MongoClient(GetConnectionString())
            .GetDatabase(MongoDbNames.Database)
            .GetCollection<TvShow>(MongoDbNames.Collections.TvShows)
            .Find(FilterDefinition<TvShow>.Empty)
            .ToListAsync();

        actualTvShows.Should().BeEquivalentTo(expectedTvShows);
    }

    [Fact]
    public async Task Find_ShouldInsertTvShow_WhenTvShowDoesNotExists()
    {
        // Arrange
        var tvShowFaker = new Faker<TvShow>();
        var expectedTvShows = tvShowFaker.GenerateTvShows(3);

        var sut = new MongoDbTvShowStorage(new MongoClient(GetConnectionString()), _loggerMock);
        await sut.SynchronizeTvShows(expectedTvShows);

        expectedTvShows = expectedTvShows.Concat(tvShowFaker.GenerateTvShows(1)).ToArray();

        // Act
        await sut.SynchronizeTvShows(expectedTvShows);

        // Assert
        var actualTvShows = await new MongoClient(GetConnectionString())
            .GetDatabase(MongoDbNames.Database)
            .GetCollection<TvShow>(MongoDbNames.Collections.TvShows)
            .Find(FilterDefinition<TvShow>.Empty)
            .ToListAsync();

        actualTvShows.Should().BeEquivalentTo(expectedTvShows);
    }

    [Fact]
    public async Task Find_ShouldDeleteTvShow_WhenTvShowDoesNotExistsInOrigin()
    {
        // Arrange
        var expectedTvShows = new Faker<TvShow>().GenerateTvShows(3);

        var sut = new MongoDbTvShowStorage(new MongoClient(GetConnectionString()), _loggerMock);
        await sut.SynchronizeTvShows(expectedTvShows);

        expectedTvShows = expectedTvShows[..2];

        // Act
        await sut.SynchronizeTvShows(expectedTvShows);

        // Assert
        var actualTvShows = await new MongoClient(GetConnectionString())
            .GetDatabase(MongoDbNames.Database)
            .GetCollection<TvShow>(MongoDbNames.Collections.TvShows)
            .Find(FilterDefinition<TvShow>.Empty)
            .ToListAsync();

        actualTvShows.Should().BeEquivalentTo(expectedTvShows);
    }
}
