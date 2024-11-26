using MongoDB.Bson.Serialization.Attributes;

namespace Codere.TvMaze.Host.Authentication;

/// <summary>
/// Represents an API key with an identifier and an active status.
/// </summary>
/// <param name="Id">
/// The unique identifier of the API key.
/// </param>
/// <param name="IsActive">
/// Indicates whether the API key is currently active.
/// This property is mapped to the "isActive" element in a BSON document.
/// </param>
public sealed record ApiKey(string Id, [property: BsonElement("isActive")] bool IsActive);