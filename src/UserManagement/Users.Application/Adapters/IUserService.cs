// <copyright file="IUserService.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Users.Application.Adapters;

/// <summary>
/// Defines the contract for user-related operations in the application layer.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Checks whether a user with the specified ID exists in the system.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to check.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains <c>true</c> if the user exists; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
