// <copyright file="IUserRepository.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel.Models.Common;
using Users.Domain.Models.Users;

namespace Users.Domain.Abstractions;

/// <summary>
/// Defines the repository contract for managing user persistence operations.
/// </summary>
/// <remarks>
/// This interface follows the Repository pattern and provides methods for
/// accessing and persisting user aggregates. Implementations should ensure
/// thread-safety and handle concurrency appropriately.
/// </remarks>
internal interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="Result{TSuccess, TError}"/> with the user if found; otherwise, an error.
    /// </returns>
    Result<User, DomainError> GetById(Guid id);

    /// <summary>
    /// Adds a new user to the repository asynchronously.
    /// </summary>
    /// <param name="user">The user aggregate to add.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="Result{TSuccess, TError}"/> with <see cref="Unit"/> on success,
    /// or an error if the operation fails (e.g., duplicate user).
    /// </returns>
    /// <remarks>
    /// This method should ensure that the user is persisted atomically.
    /// </remarks>
    Task<Result<Unit, DomainError>> AddAsync(
        User user,
        CancellationToken cancellationToken = default
    );
}
