// <copyright file="FakeUserService.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Monads.Results;
using SharedKernel.Abstractions;
using SharedKernel.Models;

namespace Users.Application.Adapters.ExistenceServiceTestApi;

/// <summary>
/// A test-double implementation of <see cref="IUserService"/> backed by an <see cref="IUsersDatabase"/>.
/// </summary>
internal sealed class FakeUserService : IUserService
{
    private readonly IUsersDatabase _database;

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeUserService"/> class.
    /// </summary>
    /// <param name="database">The user database to query. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="database"/> is null.</exception>
    public FakeUserService(IUsersDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _database = database;
        Debug.Assert(_database == database, "Database should be assigned correctly");
    }

    /// <summary>
    /// Determines whether a user with the specified identifier exists.
    /// </summary>
    /// <param name="userId">The identifier of the user to check.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains true if the user exists; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is <see cref="Guid.Empty"/>.</exception>
    public Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            cancellationToken.ThrowIfCancellationRequested();

        bool exists = _database.Query<UserEntity>().Any(u => u.UserId == userId);
        Debug.Assert(exists || !exists, "exists should be a valid boolean");

        return Task.FromResult(exists);
    }
}
