using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel;

public static class DependencyInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddScoped<IMediator, Mediator.Mediator>();
        return services;
    }
}
