using Codere.TvMaze.Mongo.Migrations;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Codere.TvMaze.Mongo
{
    internal sealed class MigrationRunner
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<BsonDocument> _versionCollection;

        public MigrationRunner(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
            _versionCollection = _database.GetCollection<BsonDocument>("migrationVersions");
        }

        public async Task RunMigrationsAsync(IEnumerable<IMigration> migrations)
        {
            using var session = await _database.Client.StartSessionAsync();

            foreach (var migration in migrations)
            {
                try
                {
                    session.StartTransaction();

                    var alreadyApplied = await _versionCollection
                        .Find(Builders<BsonDocument>.Filter.Eq("Id", migration.Id))
                        .AnyAsync();

                    if (alreadyApplied)
                    {
                        Console.WriteLine($"Migration already applied: {migration.Id}");
                        continue;
                    }

                    Console.WriteLine($"Applying migration: {migration.Id}");

                    await migration.Up(_database);

                    await _versionCollection.InsertOneAsync(new BsonDocument
                    {
                        { "Id", migration.Id },
                        { "AppliedAt", DateTime.UtcNow }
                    });

                    await session.CommitTransactionAsync();

                    Console.WriteLine($"Migration applied: {migration.Id}");
                }
                catch (Exception e)
                {
                    await session.AbortTransactionAsync();
                    Console.WriteLine($"Migration failed: {migration.Id}. Error: {e.Message}");
                    throw;
                }
            }
        }
    }
}
