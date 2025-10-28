// <copyright file="Mapper.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using WorkItems.Application.Adapters;

namespace JtlTask.WebApi.Features.Users.GetWorkItemsByAssigneeId;

/// <summary>
/// Maps between <see cref="GetWorkItemsByAssigneeIdRequest"/>, <see cref="GetWorkItemsByAssigneeIdResponse"/>, and <see cref="WorkItemEntity"/>.
/// </summary>
internal sealed class GetWorkItemsByAssigneeIdMapper
    : ResponseMapper<IEnumerable<GetWorkItemsByAssigneeIdResponse>, IEnumerable<WorkItemEntity>>
{
    public override IEnumerable<GetWorkItemsByAssigneeIdResponse> FromEntity(
        IEnumerable<WorkItemEntity> entities
    )
    {
        ArgumentNullException.ThrowIfNull(entities);

        var responses = entities
            .Select(entity =>
            {
                GetWorkItemsByAssigneeIdResponse response = new(
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

                Debug.Assert(response is not null, "Response creation must not return null.");
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
            })
            .ToList();

        return responses;
    }
}
