using System.Reflection;
using JtlTask.WebApi.Features.Users.GetUserById;
using Users.Application.Adapters;
using Users.Domain.Models.Users;
using Users.Infrastructure.Persistence;

namespace Tests.Users;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(User).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(UserEntity).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(UsersDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(GetUserByIdQuery).Assembly;
}
