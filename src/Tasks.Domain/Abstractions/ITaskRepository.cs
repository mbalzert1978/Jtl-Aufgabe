// <copyright file="ITaskRepository.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel.Models;
using SharedKernel.Models.Common;

namespace Tasks.Domain.Abstractions;

/// <summary>
/// Defines the contract for a repository that manages persistence operations for <see cref="Task"/> aggregates.
/// </summary>
/// <remarks>
/// Follows the Repository pattern. Implementations must ensure thread-safety and proper concurrency handling.
/// </remarks>
internal interface ITaskRepository
{
    /// <summary>
    /// Asynchronously retrieves a <see cref="Task"/> aggregate by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the <see cref="Task"/> to retrieve.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The result contains a <see cref="Result{TSuccess, TError}"/>
    /// with the <see cref="Task"/> if found, or a <see cref="DomainError"/> if not found or retrieval fails.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> is default.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    Task<Result<Task, DomainError>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Asynchronously adds a new <see cref="Task"/> aggregate to the repository.
    /// </summary>
    /// <param name="task">The <see cref="Task"/> aggregate to add.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The result contains a <see cref="Result{TSuccess, TError}"/>
    /// with <see cref="Unit"/> on success, or a <see cref="DomainError"/> if the operation fails (e.g., duplicate entry).
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="task"/> is null.</exception>
    /// <exception cref="OperationCanceledException">Thrown if the operation is canceled.</exception>
    /// <remarks>
    /// The operation must be atomic to ensure consistency.
    /// </remarks>
    Task<Result<Unit, DomainError>> AddAsync(
        Task task,
        CancellationToken cancellationToken = default
    );
}
