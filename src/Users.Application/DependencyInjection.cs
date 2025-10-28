using Mediator;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Abstractions;
using Users.Application.Adapters;
using Users.Application.UseCases.RegisterUser;
using Users.Domain.Abstractions;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUserApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserExistenceService, UserExistenceService>();
        // csharpier-ignore
        services.AddScoped<ICommandHandler<RegisterUserCommand, UserEntity>, RegisterUserCommandHandler>();
        return services;
    }
}
