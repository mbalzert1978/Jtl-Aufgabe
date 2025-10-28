// <copyright file="IUserExistenceService.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;

namespace SharedKernel.Abstractions;

/// <summary>
/// Domain service interface for checking user existence.
/// </summary>
/// <remarks>
/// This interface is internal to the Users module and should not be exposed
/// to other bounded contexts. External modules should use the public API
/// provided by the SharedKernel (IUserService).
/// </remarks>
internal interface IUserExistenceService
{
    /// <summary>
    /// Checks whether a user with the specified identifier exists.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="Result{TSuccess, TError}"/> indicating success or failure.
    /// </returns>
    Task<Result<Unit, IError>> VerifyUserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
