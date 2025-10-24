// <copyright file="IUserRepository.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Domain.Models.Users;
using Domain.Models.Users.ValueObjects;
using Monads.Results;
using SharedKernel.Models;
using Users.Domain;

namespace Domain.Abstractions;

/// <summary>
/// Defines the repository contract for managing user persistence operations.
/// </summary>
/// <remarks>
/// This interface follows the Repository pattern and provides methods for
/// accessing and persisting user aggregates. Implementations should ensure
/// thread-safety and handle concurrency appropriately.
/// </remarks>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the user to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="Result{TSuccess, TError}"/> with the user if found; otherwise, an error.
    /// </returns>
    Task<Result<User, DomainError>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Checks whether a user with the specified username already exists asynchronously.
    /// </summary>
    /// <param name="username">The username to check for existence.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a <see cref="Result{TSuccess, TError}"/> with <c>true</c> if a user with the username exists;
    /// otherwise, <c>false</c>. Returns an error if the operation fails.
    /// </returns>
    Task<Result<bool, DomainError>> ExistsByUsernameAsync(
        Username username,
        CancellationToken cancellationToken = default
    );

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
