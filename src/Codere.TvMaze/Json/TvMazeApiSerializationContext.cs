using System.Text.Json.Serialization;
using Codere.TvMaze.Application.Entities;

namespace Codere.TvMaze.Host.Json;

[JsonSerializable(typeof(TvShow))]
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
internal partial class TvMazeApiJsonSerializerContext : JsonSerializerContext;