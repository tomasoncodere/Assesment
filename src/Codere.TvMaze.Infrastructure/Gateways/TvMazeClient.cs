using System.Net.Http.Json;
using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Application.Features.SyncTvShows;
using Microsoft.Extensions.Logging;

namespace Codere.TvMaze.Infrastructure.Gateways;

public sealed class TvMazeClient : ITvShowInformationSource
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TvMazeClient> _logger;

    public TvMazeClient(HttpClient httpClient, ILogger<TvMazeClient> logger)
    {
        _httpClient = httpClient;

        _logger = logger;
    }

    public async Task<TvShow[]> GetTvShows()
    {
        try
        {
            var tvShows = await _httpClient.GetFromJsonAsync<TvShow[]>("https://api.tvmaze.com/shows");
            return tvShows ?? [];
        }
        catch (Exception e)
        {
            _logger.FailedToGetTvShowsInformation(e);
            throw;
        }
    }
}

internal static partial class Logs
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to get tv shows")]
    public static partial void FailedToGetTvShowsInformation(this ILogger<TvMazeClient> logger, Exception e);
}