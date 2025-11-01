// <copyright file="AssignWorkItemHandler.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Mediator;
using Monads.Results;
using Monads.Results.Extensions.Async;
using SharedKernel.Abstractions;
using WorkItems.Application.Adapters;
using WorkItems.Domain.Abstractions;
using WorkItems.Domain.Models.WorkItems;
using static SharedKernel.Models.ApplicationErrorFactory;

namespace WorkItems.Application.UseCases.AssignWorkItem;

/// <summary>
/// Handles the assignment of work items to users.
/// </summary>
internal sealed class AssignWorkItemHandler : ICommandHandler<AssignWorkItemCommand, WorkItemEntity>
{
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IUserExistenceService _userService;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignWorkItemHandler"/> class.
    /// </summary>
    /// <param name="workItemRepository">The repository for work item persistence operations.</param>
    /// <param name="userService">The user service for validating user existence.</param>
    /// <param name="timeProvider">The time provider for event timestamps.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="workItemRepository"/> or <paramref name="timeProvider"/> is null.</exception>
    public AssignWorkItemHandler(
        IWorkItemRepository workItemRepository,
        IUserExistenceService userService,
        TimeProvider timeProvider
    )
    {
        ArgumentNullException.ThrowIfNull(workItemRepository);
        ArgumentNullException.ThrowIfNull(userService);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _workItemRepository = workItemRepository;
        _userService = userService;
        _timeProvider = timeProvider;

        Debug.Assert(
            _workItemRepository == workItemRepository,
            "WorkItemRepository instance should be assigned correctly."
        );
        Debug.Assert(
            _userService == userService,
            "UserService instance should be assigned correctly."
        );
        Debug.Assert(
            _timeProvider == timeProvider,
            "TimeProvider instance should be assigned correctly."
        );
    }

    /// <summary>
    /// Handles the work item assignment command by creating and persisting a new work item.
    /// </summary>
    /// <param name="command">The assignment command containing work item details.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the created work item entity or an error.</returns>
    public async Task<Result<WorkItemEntity, IError>> HandleAsync(
        AssignWorkItemCommand command,
        CancellationToken cancellationToken
    )
    {
        Debug.Assert(command is not null, "Command must not be null.");

        return await _userService
            .VerifyUserExistsAsync(command.AssigneeId, cancellationToken)
            .BindAsync(_ =>
                WorkItem
                    .Create(
                        command.AssigneeId,
                        command.Title,
                        command.Description,
                        command.Priority,
                        command.EstimatedHours,
                        command.DueDate,
                        command.ParentTaskId,
                        command.Tags ?? [],
                        _timeProvider
                    )
                    .BindAsync(workItem =>
                        _workItemRepository
                            .AddAsync(workItem, cancellationToken)
                            .MapAsync(_ => new WorkItemEntity(
                                workItem.Id,
                                workItem.AssigneeId.Value,
                                workItem.Title.Value,
                                workItem.Description.Value,
                                workItem.Status.ToString(),
                                workItem.Priority.ToString(),
                                workItem.DueDate?.Value,
                                workItem.CompletedAt,
                                workItem.EstimatedHours.Value,
                                [.. workItem.Tags.Value.Select(tag => new WorkItemTag(tag))],
                                workItem.ParentTaskId
                            ))
                    )
                    .MapErrAsync(FromDomainError)
            )
            .ConfigureAwait(false);
    }
}
