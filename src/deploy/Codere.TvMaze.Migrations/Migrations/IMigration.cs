using MongoDB.Driver;

namespace Codere.TvMaze.Mongo.Migrations;

internal interface IMigration
{
    string Id { get; }

    Task Up(IMongoDatabase database);
}