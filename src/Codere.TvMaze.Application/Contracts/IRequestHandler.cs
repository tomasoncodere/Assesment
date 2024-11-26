namespace Codere.TvMaze.Application.Contracts;

/// <summary>
/// Defines a generic interface for handling requests and returning results.
/// </summary>
/// <typeparam name="TResult">
/// The type of the result produced by the handler.
/// </typeparam>
/// <typeparam name="TRequest">
/// The type of the request processed by the handler.
/// </typeparam>
public interface IRequestHandler<TResult, TRequest>
{
    TResult Handle(TRequest request);
}