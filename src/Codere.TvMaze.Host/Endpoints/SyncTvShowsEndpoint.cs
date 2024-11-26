using Codere.TvMaze.Application.Contracts;
using Codere.TvMaze.Application.Features.SyncTvShows;

namespace Codere.TvMaze.Host.Endpoints;

/// <summary>
/// Provides the endpoint mapping and handler for synchronizing TV shows.
/// </summary>
public static class SyncShowsEndpoint
{
    private const string Path = "/tvshows/sync";

    /// <summary>
    /// Maps the TV shows synchronization endpoint to the application routing.
    /// </summary>
    /// <param name="app">
    /// The <see cref="IEndpointRouteBuilder"/> to configure the endpoint on.
    /// </param>
    /// <returns>
    /// The same <see cref="IEndpointRouteBuilder"/> instance to allow chaining.
    /// </returns>
    public static IEndpointRouteBuilder MapSyncTvShowsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(Path, SyncTvShows)
            .RequireRateLimiting("FixedWindowPolicy")
            .RequireAuthorization();
           
        return app;
    }

    internal static IResult SyncTvShows(IRequestHandler<bool, SyncTvShowsCommand> handler) =>
        handler.Handle(new SyncTvShowsCommand())
            ? Results.Accepted()
            : Results.StatusCode(StatusCodes.Status429TooManyRequests);
}