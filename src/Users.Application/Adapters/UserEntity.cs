// <copyright file="UserEntity.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Users.Application.Adapters;

/// <summary>
/// Database model representing a user entity.
/// This is used for EF Core persistence and is separate from the application DTOs.
/// </summary>
public sealed class UserEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserEntity"/> class.
    /// Required by EF Core for materialization.
    /// </summary>
    private UserEntity() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserEntity"/> class.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="username">The username.</param>
    public UserEntity(Guid userId, string username)
    {
        Debug.Assert(userId != Guid.Empty, $"{nameof(userId)} must not be empty.");
        Debug.Assert(
            !string.IsNullOrWhiteSpace(username),
            $"{nameof(username)} must not be null or whitespace."
        );

        UserId = userId;
        Username = username;
    }
}
