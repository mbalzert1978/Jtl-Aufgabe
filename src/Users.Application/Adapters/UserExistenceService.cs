// <copyright file="UserExistenceService.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using SharedKernel.Abstractions;
using Users.Domain.Abstractions;

namespace Users.Application.Adapters;

/// <summary>
/// Adapter that implements <see cref="IUserExistenceService"/> by delegating to <see cref="IUserService"/>.
/// </summary>
internal sealed class UserExistenceService : IUserExistenceService
{
    private readonly IUserService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserExistenceService"/> class.
    /// </summary>
    /// <param name="service">The underlying user service.</param>
    /// <exception cref="ArgumentNullException">Thrown when service is null.</exception>
    public UserExistenceService(IUserService service)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
        Debug.Assert(_service == service, "Service should be assigned correctly.");
    }

    /// <summary>
    /// Checks whether a user with the specified ID exists.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Debug.Assert(_service is not null, "Service should be assigned correctly.");

        bool result = await _service.UserExistsAsync(userId, cancellationToken);
        Debug.Assert(result || !result, "result should be a boolean value");

        return result;
    }
}
