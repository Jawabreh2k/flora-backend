using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Flora.Cms.Application.Abstractions.Messaging;

/// <summary>
/// Default <see cref="ISender"/>. Builds (and caches) a strongly-typed wrapper per
/// request type that resolves the request's handler plus its pipeline behaviors and
/// composes them into an execution chain. Caching the wrapper keeps reflection off
/// the hot path — it happens once per request type for the lifetime of the process.
/// </summary>
public sealed class Sender(IServiceProvider provider) : ISender
{
    private static readonly ConcurrentDictionary<Type, object> Wrappers = new();

    public Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken ct = default)
        => Dispatch<TResponse>(command, ct);

    public Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken ct = default)
        => Dispatch<TResponse>(query, ct);

    private Task<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, CancellationToken ct)
    {
        var wrapper = (RequestHandlerWrapper<TResponse>)Wrappers.GetOrAdd(
            request.GetType(),
            static requestType => Activator.CreateInstance(
                typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(
                    requestType,
                    requestType.GetInterfaces()
                        .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                        .GetGenericArguments()[0]))!);

        return wrapper.Handle(request, provider, ct);
    }

    private abstract class RequestHandlerWrapper<TResponse>
    {
        public abstract Task<TResponse> Handle(object request, IServiceProvider sp, CancellationToken ct);
    }

    private sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
        where TRequest : IRequest<TResponse>
    {
        public override Task<TResponse> Handle(object request, IServiceProvider sp, CancellationToken ct)
        {
            var handler = sp.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle((TRequest)request, ct);

            // Wrap behaviors outermost-first: reverse so the first-registered behavior
            // ends up as the outermost link in the chain.
            foreach (var behavior in sp.GetServices<IPipelineBehavior<TRequest, TResponse>>().Reverse())
            {
                var next = pipeline;
                pipeline = () => behavior.Handle((TRequest)request, next, ct);
            }

            return pipeline();
        }
    }
}
