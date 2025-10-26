using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel;
using SharedKernel.Abstractions;
using Users.Application.Adapters;
using Users.Domain.Abstractions;
using Users.Infrastructure.Persistence;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUserInfrastructureLayer(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddDbContext<UsersDbContext>(opt => opt.UseSqlite("Data Source=.users.db"));
        services.AddScoped<IDatabase, UsersDbContext>(p => p.GetRequiredService<UsersDbContext>());
        return services;
    }

    public static async Task EnsureDatabaseCreated(
        this IServiceProvider services,
        CancellationToken ct = default
    )
    {
        Debug.Assert(services is not null, "Service provider must not be null.");

        using AsyncServiceScope scope = services.CreateAsyncScope();
        Debug.Assert(scope.ServiceProvider is not null, "Scoped service provider must not be null.");

        UsersDbContext dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        Debug.Assert(dbContext is not null, "UsersDbContext must not be null.");

        await dbContext.Database.EnsureCreatedAsync(ct).ConfigureAwait(false);
    }
}
