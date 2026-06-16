namespace Flora.Cms.Application.Abstractions.Messaging;

/// <summary>
/// Marker for any request dispatched through <see cref="ISender"/> that produces a
/// <typeparamref name="TResponse"/>. Concrete requests should implement the more
/// specific <see cref="ICommand{TResponse}"/> or <see cref="IQuery{TResponse}"/> so
/// the read/write intent (CQRS) is explicit at the call site and in the handler.
/// </summary>
public interface IRequest<out TResponse>;

/// <summary>A state-changing request (the "C" in CQRS).</summary>
public interface ICommand<out TResponse> : IRequest<TResponse>;

/// <summary>A read-only request that never mutates state (the "Q" in CQRS).</summary>
public interface IQuery<out TResponse> : IRequest<TResponse>;

/// <summary>Handles a single request type and returns its response.</summary>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken ct);
}

/// <summary>Handles a <see cref="ICommand{TResponse}"/>. Exactly one per command type.</summary>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;

/// <summary>Handles a <see cref="IQuery{TResponse}"/>. Exactly one per query type.</summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;

/// <summary>The continuation that invokes the next behavior (or the handler) in the pipeline.</summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Cross-cutting middleware wrapped around every handler (e.g. validation, logging).
/// Behaviors run in registration order, outermost first.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct);
}

/// <summary>
/// In-process dispatcher. Resolves the handler for a request from the DI container,
/// wraps it with the registered pipeline behaviors, and executes it. This is the
/// single seam the API layer uses to invoke application logic.
/// </summary>
public interface ISender
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
}
