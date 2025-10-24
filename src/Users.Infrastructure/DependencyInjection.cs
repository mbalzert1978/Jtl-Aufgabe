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
        services.AddDbContext<UsersDbContext>(opt => opt.UseInMemoryDatabase("UsersDb"));
        services.AddScoped<IDatabase, UsersDbContext>(provider =>
            provider.GetRequiredService<UsersDbContext>()
        );
        return services;
    }
}
