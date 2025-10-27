using System.Reflection;
using JtlTask.WebApi.Features.Users.GetUserById;
using WorkItems.Application.Adapters;
using WorkItems.Domain.Models.WorkItems;
using WorkItems.Infrastructure.Persistence;

namespace Tests.Users;

public abstract class BaseTest
{
    protected static readonly Assembly DomainAssembly = typeof(WorkItem).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(WorkItemEntity).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(WorkItemsDbContext).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(GetUserByIdQuery).Assembly;
}
