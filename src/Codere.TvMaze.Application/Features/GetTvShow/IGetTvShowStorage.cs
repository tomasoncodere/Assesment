using Codere.TvMaze.Application.Entities;

namespace Codere.TvMaze.Application.Features.GetTvShow;

/// <summary>
/// Defines a contract for accessing TV show data storage.
/// </summary>
public interface IGetTvShowStorage
{
    /// <summary>
    /// Retrieves a TV show by its unique identifier.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the TV show to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, containing the retrieved 
    /// <see cref="TvShow"/> or <c>null</c> if no TV show is found with the specified identifier.
    /// </returns>
    Task<TvShow?> GetTvShow(int id);
}