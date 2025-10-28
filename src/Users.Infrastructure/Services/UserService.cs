// <copyright file="UserService.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions;
using Users.Application.Adapters;

namespace Users.Infrastructure.Services;

/// <summary>
/// Service for checking user existence in the database.
/// </summary>
internal sealed class UserService : IUserService
{
    private readonly IUsersDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class.
    /// </summary>
    /// <param name="database">The users database.</param>
    /// <exception cref="ArgumentNullException">Thrown when database is null.</exception>
    public UserService(IUsersDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _database = database;
        Debug.Assert(_database == database, "Database should be assigned correctly");
    }

    /// <summary>
    /// Checks whether a user with the specified ID exists.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>True if the user exists; otherwise, false.</returns>
    public async Task<bool> UserExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        Debug.Assert(_database is not null, "Database should be assigned correctly");
        return await _database.Query<UserEntity>().AnyAsync(u => u.UserId == userId);
    }
}
