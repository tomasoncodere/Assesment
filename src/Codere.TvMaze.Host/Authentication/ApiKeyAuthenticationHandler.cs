using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Codere.TvMaze.Host.Contracts;

namespace Codere.TvMaze.Host.Authentication;

/// <summary>
/// Handles API key authentication for incoming requests.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    internal const string SchemeName = "ApiKeyScheme";
    private const string HeaderName = "X-Api-Key";
    private readonly IApiKeyStorage _apiKeyStorage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiKeyAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="options">
    /// Monitors changes to the authentication scheme options.
    /// </param>
    /// <param name="logger">
    /// The logger factory for logging information and errors.
    /// </param>
    /// <param name="encoder">
    /// The URL encoder used for security-related tasks.
    /// </param>
    /// <param name="apiKeyStorage">
    /// The storage implementation for retrieving and validating API keys.
    /// </param>
    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyStorage apiKeyStorage)
        : base(options, logger, encoder)
    {
        _apiKeyStorage = apiKeyStorage;
    }

    /// <inheritdoc/>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderName, out var extractedApiKey))
        {
            return AuthenticateResult.Fail("API Key was not provided.");
        }

        var apiKey = await _apiKeyStorage.GetActiveApiKey(extractedApiKey.ToString());

        if (apiKey is not { IsActive: true })
        {
            return AuthenticateResult.Fail("Invalid or inactive API Key.");
        }

        var claims = new[] { new Claim(ClaimTypes.Name, apiKey.Id) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

}