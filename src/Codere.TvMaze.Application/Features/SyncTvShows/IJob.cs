namespace Codere.TvMaze.Application.Features.SyncTvShows;

/// <summary>
/// Defines a contract for a job that can be executed asynchronously.
/// </summary>
public interface IJob
{
    /// <summary>
    /// Executes the job logic asynchronously.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    public Task Execute();
}