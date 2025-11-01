// <copyright file="ITaskRepository.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel.Models.Common;
using WorkItems.Domain.Models.WorkItems;

namespace WorkItems.Domain.Abstractions;

/// <summary>
/// Defines the contract for a repository that manages persistence operations for <see cref="WorkItem"/> aggregates.
/// </summary>
internal interface IWorkItemRepository
{
    /// <summary>
    /// Retrieves a work item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the work item to retrieve.</param>
    /// <returns>
    /// A result containing the work item if found, or a domain error if the work item does not exist or an error occurred.
    /// </returns>
    /// <remarks>
    /// Returns a <see cref="DomainError"/> with type 'NotFound' if no work item with the specified identifier exists.
    /// Returns a <see cref="DomainError"/> with type 'Database' if a database operation fails.
    /// </remarks>
    Result<WorkItem, DomainError> GetById(Guid id);

    /// <summary>
    /// Adds a new work item to the repository.
    /// </summary>
    /// <param name="workItem">The work item to add to the repository.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A result indicating success with <see cref="Unit"/>, or a domain error if the operation failed.
    /// </returns>
    /// <remarks>
    /// Returns a <see cref="DomainError"/> with type 'Database' if a database operation fails.
    /// The work item must have a unique identifier that does not already exist in the repository.
    /// </remarks>
    Task<Result<Unit, DomainError>> AddAsync(
        WorkItem workItem,
        CancellationToken cancellationToken = default
    );
}
