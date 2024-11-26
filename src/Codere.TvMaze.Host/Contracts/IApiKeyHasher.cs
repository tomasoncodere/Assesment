namespace Codere.TvMaze.Host.Contracts;

/// <summary>
/// Defines a contract for hashing API keys to enhance security.
/// </summary>
public interface IApiKeyHasher
{
    /// <summary>
    /// Computes a secure hash of the provided API key.
    /// </summary>
    /// <param name="apiKey">
    /// The plain-text API key to be hashed.
    /// </param>
    /// <returns>
    /// A hashed representation of the API key as a string.
    /// </returns>
    string HashApiKey(string apiKey);
}