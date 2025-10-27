using Mediator;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Behaviors;

namespace SharedKernel;

public static class DependencyInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator.Mediator>();
        services.AddTransient(typeof(ICommandPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return services;
    }
}
