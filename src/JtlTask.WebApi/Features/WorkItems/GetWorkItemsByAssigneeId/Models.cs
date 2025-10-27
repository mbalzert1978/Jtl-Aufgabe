// <copyright file="Models.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;

/// <summary>
/// Request to get work items by assignee ID.
/// </summary>
/// <param name="UserId">The unique identifier of the assignee.</param>
internal sealed record GetWorkItemsByAssigneeIdRequest(Guid UserId);

/// <summary>
/// Response containing work item information.
/// </summary>
/// <param name="Id">The unique identifier of the work item.</param>
/// <param name="AssigneeId">The unique identifier of the user assigned to this work item.</param>
/// <param name="Title">The title of the work item.</param>
/// <param name="Description">The detailed description of the work item.</param>
/// <param name="Status">The current status of the work item.</param>
/// <param name="Priority">The priority level of the work item.</param>
/// <param name="DueDate">The due date for the work item.</param>
/// <param name="CompletedAt">The date when the work item was completed.</param>
/// <param name="EstimatedHours">The estimated effort in hours.</param>
/// <param name="Tags">The tags associated with this work item.</param>
/// <param name="ParentTaskId">The identifier of the parent task if this is a subtask.</param>
internal sealed record GetWorkItemsByAssigneeIdResponse(
    Guid Id,
    Guid AssigneeId,
    string Title,
    string Description,
    string Status,
    string Priority,
    DateTimeOffset? DueDate,
    DateTimeOffset? CompletedAt,
    int EstimatedHours,
    IEnumerable<string> Tags,
    Guid ParentTaskId
);
