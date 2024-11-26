using System.Net;
using System.Net.Http.Json;
using Bogus;
using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Infrastructure.Gateways;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Codere.TvMaze.Infrastructure.Tests;

public class TvMazeClientTests
{
    private readonly Faker _faker = new();

    [Fact]
    public async Task GetTvShowsAsync_ShouldReturnTvShows_WhenRequestIsSuccessful()
    {
        // Arrange
        var expectedResult = new Faker<TvShow>()
            .CustomInstantiator(f => new TvShow(
                f.Random.Int(1, 100),
                f.Lorem.Sentence()))
            .Generate(5);

        var handler = new MockHttpMessageHandler(_ =>
            new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(expectedResult)
            });

        var sut = new TvMazeClient(
            new HttpClient(handler) { BaseAddress = new Uri(_faker.Internet.Url()) }, 
            Substitute.For<ILogger<TvMazeClient>>());

        // Act
        var actualResult = await sut.GetTvShows();

        // Assert
        actualResult.Should()
            .BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetTvShowsAsync_ShouldThrow_WhenRequestFails()
    {
        // Arrange
        var expectedMessage = _faker.Random.String();

        var handler = new MockHttpMessageHandler(_ =>
            throw new HttpRequestException(expectedMessage));
        var logger = Substitute.For<ILogger<TvMazeClient>>();

        var sut = new TvMazeClient(
            new HttpClient(handler) { BaseAddress = new Uri(_faker.Internet.Url()) },
            logger);

        // Act
        Func<Task> act = async () => await sut.GetTvShows();

        // Assert
        await act.Should()
            .ThrowAsync<HttpRequestException>()
            .WithMessage(expectedMessage);
    }
}

internal sealed class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _handlerFunc;

    public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc) =>
        _handlerFunc = handlerFunc ?? throw new ArgumentNullException(nameof(handlerFunc));

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(_handlerFunc(request));
}