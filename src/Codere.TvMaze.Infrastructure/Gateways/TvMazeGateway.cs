using Codere.TvMaze.Application.Entities;
using Codere.TvMaze.Application.Features.SyncTvShows;

namespace Codere.TvMaze.Infrastructure.Gateways
{
    public sealed class TvMazeGateway : ITvShowInformationSource
    {
        private readonly TvMazeClient _tvMazeClient;

        public TvMazeGateway(TvMazeClient tvMazeClient)
        {
            _tvMazeClient = tvMazeClient;
        }

        public Task<TvShow[]> GetTvShows() =>
            _tvMazeClient.GetTvShows();
    }
}
