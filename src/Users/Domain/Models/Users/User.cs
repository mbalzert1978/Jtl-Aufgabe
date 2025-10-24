// <copyright file="User.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Domain.Models.Users.ValueObjects;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using SharedKernel.Abstractions;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Domain.Models.Users;

/// <summary>
/// Represents a user aggregate root in the domain model.
/// Encapsulates user business logic and ensures consistency.
/// </summary>
/// <remarks>
/// This is an aggregate root following Domain-Driven Design principles.
/// Use the <see cref="UserFactory.Create"/> method to create instances.
/// </remarks>
public sealed class User : Entity, IAggregateRoot
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
    internal User(Guid id, Username username)
    {
        Debug.Assert(id != Guid.Empty, "User ID cannot be empty.");
        Debug.Assert(username is not null, "Username cannot be null.");

        Id = id;
        Username = username;

        Debug.Assert(Id == id, "User ID was not set correctly.");
        Debug.Assert(Username == username, "Username was not set correctly.");
    }

    /// <summary>
    /// Gets the string representation of the user.
    /// </summary>
    /// <returns>A string representation containing the user's ID and username.</returns>
    public override string ToString() => $"User(Id: {Id}, Username: {Username})";
}

/// <summary>
/// Factory class for creating <see cref="User"/> instances with validation.
/// </summary>
public static class UserFactory
{
    /// <summary>
    /// Creates a new <see cref="User"/> instance after validating business rules.
    /// </summary>
    /// <param name="username">The username value object for the user.</param>
    /// <returns>
    /// A <see cref="Result{T, E}"/> containing either a valid <see cref="User"/> or an error message.
    /// </returns>
    /// <remarks>
    /// This method performs business logic validation only.
    /// The username uniqueness check should be performed by the caller (repository layer).
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="username"/> is null.</exception>
    public static Result<User, string> Create(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        Result<User, string> result = UsernameFactory
            .Create(username)
            .Map(name => new User(Guid.NewGuid(), name));

        Debug.Assert(result is not null, "Result from UserFactory.Create is null.");

        return result;
    }
}
