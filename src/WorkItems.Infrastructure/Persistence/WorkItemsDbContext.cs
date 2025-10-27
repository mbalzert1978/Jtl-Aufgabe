// <copyright file="WorkItemsDbContext.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions;
using WorkItems.Application.Adapters;

namespace WorkItems.Infrastructure.Persistence;

/// <summary>
/// Database context for the WorkItems bounded context.
/// </summary>
/// <remarks>
/// This context manages the persistence of work item aggregates using Entity Framework Core.
/// Configured to use an in-memory database for development and testing purposes.
/// </remarks>
public sealed class WorkItemsDbContext : DbContext, IDatabase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemsDbContext"/> class.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public WorkItemsDbContext(DbContextOptions<WorkItemsDbContext> options)
        : base(options) { }

    /// <summary>
    /// Gets or sets the WorkItems DbSet.
    /// </summary>
    /// <value>
    /// The collection of work items in the database.
    /// </value>
    public DbSet<WorkItemEntity> WorkItems { get; set; }

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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkItemsDbContext).Assembly);
    }
}
