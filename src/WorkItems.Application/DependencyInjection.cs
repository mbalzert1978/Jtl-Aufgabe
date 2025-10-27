using Mediator;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using SharedKernel.Abstractions;
using WorkItems.Application.Adapters;
using WorkItems.Application.UseCases.AssignWorkItem;
using WorkItems.Domain.Abstractions;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkItemApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IWorkItemRepository, WorkItemRepository>();
        // csharpier-ignore
        services.AddScoped<ICommandHandler<AssignWorkItemCommand, WorkItemEntity>, AssignWorkItemHandler>();
        return services;
    }
}
