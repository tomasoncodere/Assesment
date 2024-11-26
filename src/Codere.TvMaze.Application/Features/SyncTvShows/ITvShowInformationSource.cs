using Codere.TvMaze.Application.Entities;

namespace Codere.TvMaze.Application.Features.SyncTvShows;

/// <summary>
/// Defines a contract for retrieving information about TV shows from a source.
/// </summary>
public interface ITvShowInformationSource
{
    /// <summary>
    /// Retrieves an array of TV shows from the information source.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, containing an array of <see cref="TvShow"/> objects.
    /// </returns>
    Task<TvShow[]> GetTvShows();
}