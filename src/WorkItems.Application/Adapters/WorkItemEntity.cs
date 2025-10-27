// <copyright file="TaskEntity.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Tasks.Domain.Models.Tasks;

namespace WorkItems.Application.Adapters;

/// <summary>
/// Entity Framework Core entity representing a task in the database.
/// </summary>
public sealed class WorkItemEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the task.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user assigned to this task.
    /// </summary>
    public Guid AssigneeId { get; set; }

    /// <summary>
    /// Gets or sets the title of the task.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the task.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the current status of the task.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the priority level of the task.
    /// </summary>
    public string Priority { get; set; }

    /// <summary>
    /// Gets or sets the due date for the task.
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the date when the task was completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the estimated effort in hours.
    /// </summary>
    public int EstimatedHours { get; set; }

    /// <summary>
    /// Gets or sets the tags associated with this task.
    /// </summary>
    public IEnumerable<WorkItemTag> Tags { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent task if this is a subtask.
    /// </summary>
    public Guid ParentTaskId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemEntity"/> class.
    /// </summary>
    public WorkItemEntity() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemEntity"/> class with specified values.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="assigneeId">The assignee identifier.</param>
    /// <param name="title">The task title.</param>
    /// <param name="description">The task description.</param>
    /// <param name="status">The task status.</param>
    /// <param name="priority">The task priority.</param>
    /// <param name="dueDate">The due date.</param>
    /// <param name="completedAt">The completion date.</param>
    /// <param name="estimatedHours">The estimated hours.</param>
    /// <param name="tags">The tags collection.</param>
    /// <param name="parentTaskId">The parent task identifier.</param>
    public WorkItemEntity(
        Guid id,
        Guid assigneeId,
        string title,
        string description,
        string status,
        string priority,
        DateTimeOffset? dueDate,
        DateTimeOffset? completedAt,
        int estimatedHours,
        IEnumerable<WorkItemTag> tags,
        Guid parentTaskId
    )
    {
        Debug.Assert(id != Guid.Empty, "Task ID must not be empty.");
        Debug.Assert(assigneeId != Guid.Empty, "Assignee ID must not be empty.");
        Debug.Assert(!string.IsNullOrWhiteSpace(title), "Title must not be empty.");
        Debug.Assert(estimatedHours >= 0, "Estimated hours must not be negative.");
        Debug.Assert(tags is not null, "Tags must not be null.");

        Id = id;
        AssigneeId = assigneeId;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        CompletedAt = completedAt;
        EstimatedHours = estimatedHours;
        Tags = tags;
        ParentTaskId = parentTaskId;
    }
}
