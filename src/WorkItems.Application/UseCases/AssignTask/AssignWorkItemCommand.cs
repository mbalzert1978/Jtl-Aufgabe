// <copyright file="AssignTaskCommand.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using SharedKernel.Abstractions;
using WorkItems.Application.Adapters;

namespace WorkItems.Application.UseCases.AssignTask;

/// <summary>
/// Command to assign a WorkItem to an assignee.
/// </summary>
/// <param name="AssigneeId">The unique identifier of the assignee.</param>
/// <param name="Title">The title of the task.</param>
/// <param name="EstimatedHours">The estimated hours required to complete the task.</param>
/// <param name="ParentTaskId">The unique identifier of the parent task, if any.</param>
/// <param name="Description">The description of the task.</param>
/// <param name="Priority">The priority level of the task.</param>
/// <param name="DueDate">The optional due date for the task.</param>
/// <param name="Tags">The collection of tags associated with the task.</param>
public sealed record AssignWorkItemCommand(
    Guid AssigneeId,
    string Title,
    int EstimatedHours,
    Guid ParentTaskId = default,
    string Description = "",
    string Priority = "LOW",
    DateTimeOffset? DueDate = null,
    IEnumerable<string>? Tags = null
) : ICommand<WorkItemEntity>;
