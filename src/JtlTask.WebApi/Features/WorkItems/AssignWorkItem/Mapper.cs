// <copyright file="Mapper.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using FastEndpoints;
using WorkItems.Application.Adapters;

namespace JtlTask.WebApi.Features.WorkItems.AssignWorkItem;

/// <summary>
/// Maps between <see cref="AssignWorkItemRequest"/>, <see cref="AssignWorkItemResponse"/>, and <see cref="WorkItemEntity"/>.
/// </summary>
internal sealed class AssignWorkItemMapper
    : Mapper<AssignWorkItemRequest, AssignWorkItemResponse, WorkItemEntity>
{
    /// <summary>
    /// Converts a <see cref="WorkItemEntity"/> to a <see cref="AssignWorkItemResponse"/>.
    /// </summary>
    /// <param name="entity">The entity to convert.</param>
    /// <returns>A <see cref="AssignWorkItemResponse"/> representing the entity data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public override AssignWorkItemResponse FromEntity(WorkItemEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        AssignWorkItemResponse response = new(
            entity.Id,
            entity.AssigneeId,
            entity.Title,
            entity.Description,
            entity.Status,
            entity.Priority,
            entity.DueDate,
            entity.CompletedAt,
            entity.EstimatedHours,
            entity.Tags?.Select(t => t.Name) ?? [],
            entity.ParentTaskId
        );

        Debug.Assert(response.Id == entity.Id, "Mapped response ID must match entity Id.");
        Debug.Assert(
            response.AssigneeId == entity.AssigneeId,
            "Mapped response AssigneeId must match entity AssigneeId."
        );
        Debug.Assert(
            response.Title == entity.Title,
            "Mapped response Title must match entity Title."
        );
        Debug.Assert(
            response.Description == entity.Description,
            "Mapped response Description must match entity Description."
        );
        Debug.Assert(
            response.Status == entity.Status,
            "Mapped response Status must match entity Status."
        );
        Debug.Assert(
            response.Priority == entity.Priority,
            "Mapped response Priority must match entity Priority."
        );
        Debug.Assert(
            response.EstimatedHours == entity.EstimatedHours,
            "Mapped response EstimatedHours must match entity EstimatedHours."
        );

        return response;
    }
}
