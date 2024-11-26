using Bogus;
using Codere.TvMaze.Application.Entities;

namespace Codere.TvMaze.Infrastructure.Tests;

internal static class FakesGenerator
{
    public static TvShow[] GenerateTvShows(this Faker<TvShow> faker, int count) =>
        faker
            .CustomInstantiator(f => new TvShow(f.Random.Int(1, 100), f.Lorem.Sentence()))
            .Generate(count)
            .ToArray();
}