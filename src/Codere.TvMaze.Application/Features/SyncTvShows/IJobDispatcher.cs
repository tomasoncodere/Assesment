namespace Codere.TvMaze.Application.Features.SyncTvShows;

/// <summary>
/// Defines a contract for dispatching jobs for execution.
/// </summary>
public interface IJobDispatcher
{
    /// <summary>
    /// Dispatches a specified job for execution.
    /// </summary>
    /// <param name="job">
    /// The job to be dispatched. Must implement the <see cref="IJob"/> interface.
    /// </param>
    /// <returns>
    /// <c>true</c> if the job was successfully dispatched; otherwise, <c>false</c>.
    /// </returns>
    bool Dispatch(IJob job);
}