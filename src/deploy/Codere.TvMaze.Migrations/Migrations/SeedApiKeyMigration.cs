using System.Security.Cryptography;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;

namespace Codere.TvMaze.Mongo.Migrations
{
    public class CreateTvShowsMigration : IMigration
    {
        public string Id => "1_SeedKeys";

        public async Task Up(IMongoDatabase database)
        {
            var collectionName = "apiKeys";

            var collectionNames = await database.ListCollectionNamesAsync();
            var exists = await collectionNames.ToListAsync();
            if (!exists.Contains(collectionName))
            {
                await database.CreateCollectionAsync(collectionName);
            }

            var collection = database.GetCollection<BsonDocument>(collectionName);

            var apiKeys = new[]
            {
                new BsonDocument
                {
                    { "_id", HashApiKey("key-1-active") },
                    { "isActive", true }
                },
                new BsonDocument
                {
                    { "_id", HashApiKey("key-2-active") },
                    { "isActive", true }
                },
                new BsonDocument
                {
                    { "_id", HashApiKey("key-3-inactive") },
                    { "isActive", false }
                }
            };

            await collection.InsertManyAsync(apiKeys);

            Console.WriteLine("Inserted 3 API keys (2 active, 1 inactive).");
        }

        private static string HashApiKey(string apiKey) =>
            Convert.ToBase64String(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(apiKey)));
    }
}
