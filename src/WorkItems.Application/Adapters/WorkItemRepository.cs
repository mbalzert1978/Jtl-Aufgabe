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
    private readonly IWorkItemsDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemRepository"/> class.
    /// </summary>
    /// <param name="database">The database instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when database is null.</exception>
    public WorkItemRepository(IWorkItemsDatabase database)
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
                [.. workItem.Tags.Value.Select(tag => new WorkItemTag(tag))],
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
    public Result<WorkItem, DomainError> GetById(Guid id)
    {
        try
        {
            WorkItemEntity? entity = _database
                .Query<WorkItemEntity>()
                .FirstOrDefault(w => w.Id == id);

            if (entity is null)
                return Failure<WorkItem, DomainError>(NotFound(id, typeof(WorkItemEntity)));

            Debug.Assert(entity.Id == id, "Retrieved entity ID does not match requested ID.");

            var workItem = WorkItem.Rehydrate(
                entity.Id,
                entity.AssigneeId,
                entity.Title,
                entity.Description,
                entity.Status,
                entity.Priority,
                entity.DueDate,
                entity.CompletedAt,
                entity.EstimatedHours,
                entity.Tags.Select(t => t.Name),
                entity.ParentTaskId
            );

            return Success<WorkItem, DomainError>(workItem);
        }
        catch (Exception exc)
        {
            DomainError error = DatabaseError(exc);
            return Failure<WorkItem, DomainError>(error);
        }
    }
}
