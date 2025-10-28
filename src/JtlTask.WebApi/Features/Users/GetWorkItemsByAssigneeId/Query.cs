// <copyright file="Query.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using WorkItems.Application.Adapters;

namespace JtlTask.WebApi.Features.Users.GetWorkItemsByAssigneeId;

/// <summary>
/// Represents a query to retrieve work items by assignee ID.
/// </summary>
/// <param name="AssigneeId">The unique identifier of the assignee.</param>
internal sealed record GetWorkItemsByAssigneeIdQuery(Guid AssigneeId)
    : ICommand<IEnumerable<WorkItemEntity>>;
