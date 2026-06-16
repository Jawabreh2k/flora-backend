using System.Reflection;
using FluentValidation;
using Flora.Cms.Application.Abstractions.Contact;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Common.Behaviors;
using Flora.Cms.Application.Features.Contact;
using Microsoft.Extensions.DependencyInjection;

namespace Flora.Cms.Application;

/// <summary>Composition root for the Application layer (CQRS handlers, behaviors, validators).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<ISender, Sender>();
        services.AddScoped<IContactRecipientResolver, ContactRecipientResolver>();

        RegisterHandlers(services, assembly);

        // Behaviors run in registration order, outermost first: log the whole request,
        // then validate before the handler runs.
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        return services;
    }

    /// <summary>
    /// Registers every concrete <see cref="IRequestHandler{TRequest,TResponse}"/> in the
    /// assembly against its closed handler interface so the dispatcher can resolve it.
    /// </summary>
    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        foreach (var type in assembly.GetTypes().Where(t => t is { IsAbstract: false, IsInterface: false }))
        {
            var handlerInterfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
                services.AddScoped(handlerInterface, type);
        }
    }
}
