using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using WorkItems.Infrastructure.Persistence;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkItemInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<WorkItemsDbContext>(opt =>
            opt.UseSqlite(configuration.GetConnectionString("WorkItemsDatabase"))
        );
        services.AddScoped<IWorkItemsDatabase>(p => p.GetRequiredService<WorkItemsDbContext>());
        return services;
    }

    public static async Task EnsureWorkItemsDatabaseCreated(
        this IServiceProvider services,
        CancellationToken ct = default
    )
    {
        Debug.Assert(services is not null, "Service provider must not be null.");

        using AsyncServiceScope scope = services.CreateAsyncScope();
        Debug.Assert(
            scope.ServiceProvider is not null,
            "Scoped service provider must not be null."
        );

        WorkItemsDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>();
        Debug.Assert(dbContext is not null, "UsersDbContext must not be null.");

        bool canConnect = await dbContext.Database.CanConnectAsync(ct).ConfigureAwait(false);

        if (!canConnect)
            await dbContext.Database.EnsureCreatedAsync(ct).ConfigureAwait(false);
    }
}
