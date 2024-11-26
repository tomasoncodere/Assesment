using System.Text.Encodings.Web;
using Bogus;
using Codere.TvMaze.Host.Authentication;
using Codere.TvMaze.Host.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Codere.TvMaze.Infrastructure.Tests;

public class ApiKeyAuthenticationHandlerTests
{
    private const string TestAuthenticationScheme = "TestAuthenticationScheme";
    private const string AuthenticationHeaderName = "X-Api-Key";

    private readonly IApiKeyStorage _apiKeyStorageMock;
    private readonly ApiKeyAuthenticationHandler _sut;
    private readonly Faker _faker;

    public ApiKeyAuthenticationHandlerTests()
    {
        _faker = new Faker();
        _apiKeyStorageMock = Substitute.For<IApiKeyStorage>();

        var optionsMock = Substitute.For<IOptionsMonitor<AuthenticationSchemeOptions>>();
        optionsMock.Get(Arg.Any<string>()).Returns(new AuthenticationSchemeOptions());
        var loggerFactoryMock = Substitute.For<ILoggerFactory>();
        var encoder = Substitute.For<UrlEncoder>();

        _sut = new ApiKeyAuthenticationHandler(
            optionsMock,
            loggerFactoryMock,
            encoder,
            _apiKeyStorageMock
        );
    }

    [Fact]
    public async Task Authenticate_ShouldReturnFail_WhenApiKeyHeaderIsMissing()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var scheme = new AuthenticationScheme(TestAuthenticationScheme, null, typeof(ApiKeyAuthenticationHandler));
        await _sut.InitializeAsync(scheme, context);

        // Act
        var actualResult = await _sut.AuthenticateAsync();

        // Assert
        actualResult.Failure.Should().NotBeNull();
    }

    [Fact]
    public async Task Authenticate_ShouldReturnFail_WhenApiKeyNotFound()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers[AuthenticationHeaderName] = _faker.Random.String();
        var scheme = new AuthenticationScheme(TestAuthenticationScheme, null, typeof(ApiKeyAuthenticationHandler));
        await _sut.InitializeAsync(scheme, context);

        _apiKeyStorageMock
            .GetActiveApiKey(Arg.Any<string>())
            .ReturnsNull();

        // Act
        var actualResult = await _sut.AuthenticateAsync();

        // Assert
        actualResult.Failure.Should().NotBeNull();
    }

    [Fact]
    public async Task Authenticate_ShouldReturnFail_WhenApiKeyNotActive()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var inactiveKey = _faker.Random.Hash();
        context.Request.Headers[AuthenticationHeaderName] = inactiveKey;
        var scheme = new AuthenticationScheme(TestAuthenticationScheme, null, typeof(ApiKeyAuthenticationHandler));
        await _sut.InitializeAsync(scheme, context);

        _apiKeyStorageMock
            .GetActiveApiKey(inactiveKey)
            .Returns(new ApiKey(inactiveKey, false));

        // Act
        var actualResult = await _sut.AuthenticateAsync();

        // Assert
        actualResult.Failure.Should().NotBeNull();
    }

    [Fact]
    public async Task Authenticate_ShouldReturnSuccess_WhenApiKeyIsValid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var validKey = _faker.Random.Hash();
        context.Request.Headers[AuthenticationHeaderName] = validKey;
        var scheme = new AuthenticationScheme(TestAuthenticationScheme, null, typeof(ApiKeyAuthenticationHandler));
        await _sut.InitializeAsync(scheme, context);

        _apiKeyStorageMock
            .GetActiveApiKey(validKey)
            .Returns(new ApiKey(validKey, true));

        // Act
        var actualResult = await _sut.AuthenticateAsync();

        // Assert
        actualResult.Succeeded.Should().BeTrue();
        actualResult.Ticket.Should().NotBeNull();
        actualResult.Ticket!.Principal.Claims.Should().Contain(c =>
            c.Subject != null
            && string.Equals(c.Subject.Name, validKey)
            && c.Subject.IsAuthenticated);
    }

}