// <copyright file="IDatabase.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Abstractions;

/// <summary>
/// Defines the contract for database operations.
/// </summary>
public interface IDatabase
{
    /// <summary>
    /// Retrieves a queryable collection of entities of the specified type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to retrieve.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> of the specified entity type.</returns>
    IQueryable<TEntity> Query<TEntity>()
        where TEntity : class;

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add.</typeparam>
    /// <param name="entity">The entity instance to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class;
}

/// <summary>
/// Represents the database operations specific to user entities.
/// </summary>
public interface IUsersDatabase : IDatabase;

/// <summary>
/// Represents the database operations specific to work item entities.
/// </summary>
public interface IWorkItemsDatabase : IDatabase;
