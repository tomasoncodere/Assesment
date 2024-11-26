namespace Codere.TvMaze.Application.Entities;

/// <summary>
/// Represents a TV show with an identifier and a name.
/// </summary>
/// <param name="Id">
/// The unique identifier for the TV show.
/// </param>
/// <param name="Name">
/// The name of the TV show.
/// </param>
public sealed record TvShow(int Id, string Name);