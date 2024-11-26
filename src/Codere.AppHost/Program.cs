var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("Mongo", 60800);
var mongoDb = mongo.AddDatabase("TvMaze");

var migrations = builder
    .AddProject<Projects.Codere_TvMaze_Mongo>("Mongo-TvMaze")
    .WithReference(mongoDb)
    .WaitFor(mongoDb);

builder.AddProject<Projects.Codere_TvMaze_Host>("Api-TvMaze")
    .WithReference(mongoDb)
    .WaitFor(mongoDb)
    .WaitForCompletion(migrations);

builder.Build().Run();
