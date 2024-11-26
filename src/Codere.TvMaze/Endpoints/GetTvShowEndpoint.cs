using System.Net;
using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Application.Contracts;
using Codere.TvMaze.Application.Features.GetTvShow;

namespace Codere.TvMaze.Host.Endpoints;

/// <summary>
/// Provides the endpoint mapping and handler for retrieving TV show details.
/// </summary>
public static class GetTvShowEndpoint
{
    private const string Path = "tvshows/{tvShowId}";

    /// <summary>
    /// Maps the TV show retrieval endpoint to the application routing.
    /// </summary>
    /// <param name="app">
    /// The <see cref="IEndpointRouteBuilder"/> to configure the endpoint on.
    /// </param>
    /// <returns>
    /// The same <see cref="IEndpointRouteBuilder"/> instance to allow chaining.
    /// </returns>
    public static IEndpointRouteBuilder MapGetTvShowEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(Path, GetTvShow);
        return app;
    }
    internal static async Task<IResult> GetTvShow(int tvShowId, IRequestHandler<Task<TvShow?>, GetTvShowQuery> handler)
    {
        var tvShow = await handler.Handle(new GetTvShowQuery(tvShowId));
        if (tvShow is null)
        {
            return Results.Problem(
                detail: $"TV show with ID {tvShowId} was not found.",
                statusCode: (int) HttpStatusCode.NotFound,
                title: "Resource Not Found",
                type: "https://httpstatuses.com/404"
            );
        }

        return TypedResults.Ok(tvShow);
    }
}