// <copyright file="UsersDbContext.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions;
using Users.Application.Adapters;

namespace Users.Infrastructure.Persistence;

/// <summary>
/// Database context for the Users bounded context.
/// </summary>
/// <remarks>
/// This context manages the persistence of user aggregates using Entity Framework Core.
/// Configured to use an in-memory database for development and testing purposes.
/// </remarks>
public sealed class UsersDbContext : DbContext, IUsersDatabase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UsersDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options) { }

    /// <summary>
    /// Gets or sets the Users DbSet.
    /// </summary>
    /// <value>
    /// The collection of users in the database.
    /// </value>
    public DbSet<UserEntity> Users { get; set; }

    /// <summary>
    /// Retrieves a queryable collection of entities of the specified type.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to retrieve.</typeparam>
    /// <returns>An <see cref="IQueryable{T}"/> of the specified entity type.</returns>
    public IQueryable<TEntity> Query<TEntity>()
        where TEntity : class => Set<TEntity>();

    /// <summary>
    /// Adds an entity to the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity to add.</typeparam>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    async Task IDatabase.AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await Set<TEntity>().AddAsync(entity);

        int rowsAffected = await SaveChangesAsync(cancellationToken);
        Debug.Assert(rowsAffected > 0, "Entity must have been persisted to the database.");
    }

    /// <summary>
    /// Configures the entity models using the model builder.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
    }
}
