using System.Threading.RateLimiting;
using Codere.ServiceDefaults;
using Codere.TvMaze.Application.Contracts;
using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Application.Features.GetTvShow;
using Codere.TvMaze.Application.Features.SyncTvShows;
using Codere.TvMaze.Host;
using Codere.TvMaze.Host.Authentication;
using Codere.TvMaze.Host.Contracts;
using Codere.TvMaze.Host.Endpoints;
using Codere.TvMaze.Host.Infrastructure;
using Codere.TvMaze.Infrastructure.Gateways;
using Microsoft.AspNetCore.Authentication;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddMongoDBClient("TvMaze");
builder.AddTvMaze();

builder.Services
    .AddScoped<IApiKeyStorage>(s =>
        new HashedApiKeyStorage(
            new MongoDbApiKeyStorage(
                s.GetRequiredService<IMongoClient>(),
                s.GetRequiredService<ILogger<MongoDbApiKeyStorage>>()),
            s.GetRequiredService<IApiKeyHasher>()))
    .AddSingleton<IApiKeyHasher, SHA256ApiKeyHasher>()
    .AddAuthorization()
    .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
        .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, null);

builder.Services.AddHttpClient<TvMazeClient>();
    
var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapGetTvShowEndpoint();
app.MapSyncTvShowsEndpoint();

app.Run();