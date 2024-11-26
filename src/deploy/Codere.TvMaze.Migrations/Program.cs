using Codere.TvMaze.Mongo;
using Codere.TvMaze.Mongo.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder();
builder.AddMongoDBClient("TvMaze");

var app = builder.Build();

var migrationRunner = new MigrationRunner(app.Services.GetRequiredService<IMongoClient>(), "TvMaze");
await migrationRunner.RunMigrationsAsync(
[
    new CreateTvShowsMigration()
]);