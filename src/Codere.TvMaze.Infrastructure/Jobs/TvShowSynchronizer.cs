using Codere.TvMaze.Application.Features.SyncTvShows;

namespace Codere.TvMaze.Infrastructure.Jobs
{
    public class TvShowSynchronizer : IJob
    {
        private readonly ITvShowInformationSource _tvShowInformationSource;
        private readonly ISyncTvShowsStorage _syncTvShowStorage;

        public TvShowSynchronizer(ITvShowInformationSource tvShowInformationSource, ISyncTvShowsStorage syncTvShowStorage)
        {
            _tvShowInformationSource = tvShowInformationSource;
            _syncTvShowStorage = syncTvShowStorage;
        }

        public async Task Execute()
        {
            var tvShows = await _tvShowInformationSource.GetTvShows();
            await _syncTvShowStorage.SynchronizeTvShows(tvShows);
        }
    }
}
