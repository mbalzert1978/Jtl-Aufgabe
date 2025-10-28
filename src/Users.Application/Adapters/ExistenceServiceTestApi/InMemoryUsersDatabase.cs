// <copyright file="InMemoryUsersDatabase.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Diagnostics;
using SharedKernel.Abstractions;

namespace Users.Application.Adapters.ExistenceServiceTestApi;

/// <summary>
/// In-memory implementation of <see cref="IUsersDatabase"/> for testing purposes.
/// </summary>
/// <remarks>
/// Provides a thread-safe in-memory database implementation following Martin Fowler's
/// TestFramework pattern. Used as a test double for moving parts while keeping real
/// implementations for services and business logic.
/// </remarks>
internal sealed class InMemoryUsersDatabase : IUsersDatabase
{
    private readonly ConcurrentDictionary<Type, ConcurrentBag<object>> _storage = new();

    /// <summary>
    /// Retrieves a queryable collection of entities of the specified type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to retrieve.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> of the specified entity type.</returns>
    public IQueryable<TEntity> Query<TEntity>()
        where TEntity : class
    {
        if (!_storage.TryGetValue(typeof(TEntity), out ConcurrentBag<object>? bag))
            return Enumerable.Empty<TEntity>().AsQueryable();

        Debug.Assert(bag is not null, "Storage bag should not be null when retrieved.");
        return bag.Cast<TEntity>().AsQueryable();
    }

    /// <summary>
    /// Adds a new entity to the in-memory database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add.</typeparam>
    /// <param name="entity">The entity instance to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entity);

        Type entityType = typeof(TEntity);
        ConcurrentBag<object> bag = _storage.GetOrAdd(entityType, _ => []);
        bag.Add(entity);

        Debug.Assert(
            _storage.ContainsKey(entityType),
            "Entity type should be in storage after adding."
        );
        Debug.Assert(bag.Contains(entity), "Entity should be in storage bag after adding.");

        return Task.CompletedTask;
    }

    /// <summary>
    /// Clears all data from the in-memory database.
    /// </summary>
    /// <remarks>
    /// Useful for resetting state between tests.
    /// </remarks>
    public void Clear()
    {
        _storage.Clear();
        Debug.Assert(_storage.IsEmpty, "Storage should be empty after clearing.");
    }
}
