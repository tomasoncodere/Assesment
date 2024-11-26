using Codere.TvMaze.Host.Authentication;
using Codere.TvMaze.Host.Contracts;

namespace Codere.TvMaze.Host.Infrastructure;

/// <summary>
/// Decorates an existing <see cref="IApiKeyStorage"/> implementation to support hashed API key lookups.
/// </summary>
public class HashedApiKeyStorage : IApiKeyStorage
{
    private readonly IApiKeyStorage _innerStorage;
    private readonly IApiKeyHasher _apiKeyHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="HashedApiKeyStorage"/> class.
    /// </summary>
    /// <param name="innerStorage">
    /// The underlying <see cref="IApiKeyStorage"/> implementation to delegate API key retrieval to.
    /// </param>
    /// <param name="apiKeyHasher">
    /// An implementation of <see cref="IApiKeyHasher"/> used to hash API keys before lookup.
    /// </param>
    public HashedApiKeyStorage(IApiKeyStorage innerStorage, IApiKeyHasher apiKeyHasher)
    {
        _innerStorage = innerStorage;
        _apiKeyHasher = apiKeyHasher;
    }

    /// <inheritdoc cref="IApiKeyStorage.GetActiveApiKey(string)"/>>
    public async Task<ApiKey?> GetActiveApiKey(string apiKeyId) =>
        await _innerStorage.GetActiveApiKey(_apiKeyHasher.HashApiKey(apiKeyId));
}