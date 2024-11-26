namespace Codere.TvMaze.Infrastructure.Storage;

public static class MongoDbNames
{
    public static string Database => "TvMaze";

    public static class Collections
    {
        public static string TvShows => "tvShows";

        public static string ApiKeys => "apiKeys";
    }
}