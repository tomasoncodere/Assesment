using Codere.TvMaze.Application.Entities;

namespace Codere.TvMaze.Application.Features.SyncTvShows;

/// <summary>
/// Defines a contract for synchronizing a collection of TV shows with the storage system.
/// </summary>
public interface ISyncTvShowsStorage
{
    /// <summary>
    /// Synchronizes the specified collection of TV shows with the storage system.
    /// </summary>
    /// <param name="tvShows">
    /// An array of <see cref="TvShow"/> objects to be synchronized with the storage system.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    Task SynchronizeTvShows(TvShow[] tvShows);
}