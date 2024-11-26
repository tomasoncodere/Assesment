using System.Security.Cryptography;
using System.Text;
using Codere.TvMaze.Host.Contracts;

namespace Codere.TvMaze.Host.Infrastructure;

/// <summary>
/// Provides an implementation of <see cref="IApiKeyHasher"/> that hashes API keys using the SHA-256 algorithm.
/// </summary>
public class SHA256ApiKeyHasher : IApiKeyHasher
{
    /// <summary>
    /// Computes an SHA-256 hash of the provided API key and encodes it as a Base64 string.
    /// </summary>
    /// <param name="apiKey">
    /// The plain-text API key to be hashed.
    /// </param>
    /// <returns>
    /// A Base64-encoded string representing the SHA-256 hash of the API key.
    /// </returns>
    public string HashApiKey(string apiKey) =>
        Convert.ToBase64String(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(apiKey)));
}