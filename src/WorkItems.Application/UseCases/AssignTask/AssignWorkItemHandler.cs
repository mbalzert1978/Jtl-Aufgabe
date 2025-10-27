using Monads.Results;
using SharedKernel.Abstractions;
using WorkItems.Application.Adapters;
using WorkItems.Application.UseCases.AssignTask;

namespace Tasks.Application.UseCases.AssignTask;

public sealed class AssignWorkItemHandler : ICommandHandler<AssignWorkItemCommand, WorkItemEntity>
{
    public Task<Result<WorkItemEntity, IError>> HandleAsync(
        AssignWorkItemCommand command,
        CancellationToken cancellationToken
    )
    {
        throw new NotImplementedException();
    }
}
