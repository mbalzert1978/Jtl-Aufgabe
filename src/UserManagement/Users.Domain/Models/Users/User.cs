// <copyright file="User.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using SharedKernel.Abstractions;
using SharedKernel.Models.Common;
using Users.Domain.Models.Users.ValueObjects;

namespace Users.Domain.Models.Users;

/// <summary>
/// Represents a user aggregate root in the domain model.
/// Encapsulates user business logic and ensures consistency.
/// </summary>
/// <remarks>
/// This is an aggregate root following Domain-Driven Design principles.
/// </remarks>
internal sealed class User : Entity, IAggregateRoot
{
    /// <summary>
    /// Gets the username of the user.
    /// </summary>
    /// <value>
    /// A validated <see cref="Username"/> value object.
    /// </value>
    public Username Username { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the user.</param>
    /// <param name="username">The username value object.</param>
    /// <remarks>
    /// This constructor is internal to enforce the use of the factory method while allowing test access.
    /// </remarks>
    private User(Guid id, Username username)
    {
        Debug.Assert(id != Guid.Empty, "User ID cannot be empty.");
        Debug.Assert(username is not null, "Username cannot be null.");

        Id = id;
        Username = username;

        Debug.Assert(Id == id, "User ID should be assigned correctly.");
        Debug.Assert(Username == username, "Username should be assigned correctly.");
    }

    /// <summary>
    /// Creates a new <see cref="User"/> instance after validating business rules.
    /// </summary>
    /// <param name="username">The username value object for the user.</param>
    /// <param name="timeProvider">The time provider for event timestamps.</param>
    /// <returns>
    /// A <see cref="Result{T, E}"/> containing either a valid <see cref="User"/> or an error message.
    /// </returns>
    /// <remarks>
    /// This method performs business logic validation only.
    /// The username uniqueness check should be performed by the caller (repository layer).
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="username"/> is null.</exception>
    public static Result<User, DomainError> Create(string username, TimeProvider timeProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        Result<User, DomainError> result = UsernameFactory
            .Create(username)
            .Map(name => new User(Guid.NewGuid(), name));

        Debug.Assert(result is not null, "Result from UserFactory.Create is null.");

        return result;
    }

    /// <summary>
    /// Rehydrates an existing <see cref="User"/> from persistence data.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="username">The username string.</param>
    /// <returns>A <see cref="User"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is empty.</exception>
    public static User Rehydrate(Guid id, string username)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.");

        return new(id, new(username));
    }
}
