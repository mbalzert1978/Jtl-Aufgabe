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
    /// Gets or sets the collection of task references associated with the user.
    /// </summary>
    public IEnumerable<UserTaskReference> Tasks { get; set; }

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
    /// <param name="tasks">The collection of task references associated with the user.</param>
    public UserEntity(Guid userId, string username, IEnumerable<UserTaskReference> tasks)
    {
        Debug.Assert(userId != Guid.Empty, $"{nameof(userId)} must not be empty.");
        Debug.Assert(
            !string.IsNullOrWhiteSpace(username),
            $"{nameof(username)} must not be null or whitespace."
        );

        UserId = userId;
        Username = username;
        Tasks = tasks;

    }
}

/// <summary>
/// Database model representing a reference from a user to a task in another database.
/// </summary>
public sealed class UserTaskReference
{
    /// <summary>
    /// Gets or sets the task identifier.
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserTaskReference"/> class.
    /// Required by EF Core for materialization.
    /// </summary>
    private UserTaskReference() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserTaskReference"/> class.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    public UserTaskReference(Guid taskId)
    {
        Debug.Assert(taskId != Guid.Empty, $"{nameof(taskId)} must not be empty.");
        TaskId = taskId;
    }
}
