using Codere.TvMaze.Host.Authentication;

namespace Codere.TvMaze.Host.Contracts;

/// <summary>
/// Defines a contract for accessing and managing API key storage.
/// </summary>
public interface IApiKeyStorage
{
    /// <summary>
    /// Retrieves an active API key by its unique identifier.
    /// </summary>
    /// <param name="apiKeyId">
    /// The unique identifier of the API key to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation, containing the <see cref="ApiKey"/> 
    /// if found and active, or <c>null</c> if not found or inactive.
    /// </returns>
    public Task<ApiKey?> GetActiveApiKey(string apiKeyId);
}