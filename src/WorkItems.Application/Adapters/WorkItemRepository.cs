// <copyright file="WorkItemRepository.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Abstractions;
using SharedKernel.Models.Common;
using WorkItems.Domain.Abstractions;
using WorkItems.Domain.Models.WorkItems;
using static Monads.Results.ResultFactory;
using static SharedKernel.Models.Common.DomainErrorFactory;

namespace WorkItems.Application.Adapters;

/// <summary>
/// Repository implementation for managing work item entities in the database.
/// </summary>
internal sealed class WorkItemRepository : IWorkItemRepository
{
    private readonly IDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when database is null.</exception>
    public WorkItemRepository(IDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);

        _database = database;

        Debug.Assert(_database == database, "Database instance was not set correctly.");
    }

    /// <inheritdoc/>
    public async Task<Result<Unit, DomainError>> AddAsync(
        WorkItem workItem,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            WorkItemEntity entity = new(
                workItem.Id,
                workItem.AssigneeId.Value,
                workItem.Title.Value,
                workItem.Description.Value,
                workItem.Status.ToString(),
                workItem.Priority.ToString(),
                workItem.DueDate?.Value,
                workItem.CompletedAt,
                workItem.EstimatedHours.Value,
                workItem.Tags.Value,
                workItem.ParentTaskId
            );
            Debug.Assert(entity.Id == workItem.Id, "WorkItemEntity ID does not match");

            await _database.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            DomainError error = DatabaseError(exc);
            return Failure<Unit, DomainError>(error);
        }
        return Success<Unit, DomainError>(default);
    }

    /// <inheritdoc/>
    public async Task<Result<WorkItem, DomainError>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return (await _database.GetAsync<WorkItemEntity>(cancellationToken)).FirstOrDefault(w =>
                w.Id == id
            ) switch
            {
                WorkItemEntity e => Success<WorkItem, DomainError>(
                    WorkItem.Rehydrate(
                        e.Id,
                        e.AssigneeId,
                        e.Title,
                        e.Description,
                        e.Status,
                        e.Priority,
                        e.DueDate,
                        e.CompletedAt,
                        e.EstimatedHours,
                        e.Tags,
                        e.ParentTaskId
                    )
                ),
                _ => Failure<WorkItem, DomainError>(Generic("Work item not found.")),
            };
        }
        catch (Exception exc)
        {
            DomainError error = DatabaseError(exc);
            return Failure<WorkItem, DomainError>(error);
        }
    }
}
