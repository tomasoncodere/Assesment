using Codere.TvMaze.Application.Contracts;
using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Application.Features.GetTvShow;
using Codere.TvMaze.Application.Features.SyncTvShows;
using Codere.TvMaze.Infrastructure.Gateways;
using Codere.TvMaze.Infrastructure.Jobs;
using Codere.TvMaze.Infrastructure.Storage;

namespace Codere.TvMaze.Host;

/// <summary>
/// Provides extension methods for configuring services related to TV show functionality.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Configures services required for TV show management and synchronization in the application.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder"/> to which services will be added.
    /// </param>
    /// <returns>
    /// The same <see cref="IHostApplicationBuilder"/> instance to allow chaining.
    /// </returns>
    public static IHostApplicationBuilder AddTvMaze(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddScoped<IGetTvShowStorage, MongoDbTvShowStorage>()
            .AddScoped<ISyncTvShowsStorage, MongoDbTvShowStorage>()
            .AddTransient<ITvShowInformationSource, TvMazeGateway>()
            .AddTransient<IJob, TvShowSynchronizer>()
            .AddSingleton<IJobDispatcher, JobQueue>()
            .AddScoped<IRequestHandler<Task<TvShow?>, GetTvShowQuery>, GetTvShowQueryHandler>()
            .AddScoped<IRequestHandler<bool, SyncTvShowsCommand>, SyncTvShowsCommandHandler>()
            .AddHostedService(s => (JobQueue)s.GetRequiredService<IJobDispatcher>());

        return builder;
    }
}