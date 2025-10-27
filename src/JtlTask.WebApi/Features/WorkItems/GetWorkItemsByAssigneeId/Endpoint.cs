// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using FastEndpoints;
using WorkItems.Application.Adapters;

namespace JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;

/// <summary>
/// Endpoint for retrieving work items assigned to a specific user.
/// </summary>
internal sealed class Endpoint
    : Endpoint<
        GetWorkItemsByAssigneeIdRequest,
        IEnumerable<GetWorkItemsByAssigneeIdResponse>,
        GetWorkItemsByAssigneeIdMapper
    >
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        AllowAnonymous();
        Get("api/v1/users/{userId:guid}/workitems");

        Summary(s =>
        {
            Debug.Assert(s is not null, "Summary configuration object cannot be null.");

            s.Summary = "Get work items by assignee ID";
            s.Description = "Retrieves all work items assigned to a specific user";
            s.Response<IEnumerable<GetWorkItemsByAssigneeIdResponse>>(
                StatusCodes.Status200OK,
                "Work items successfully retrieved"
            );
            s.Response(
                StatusCodes.Status200OK,
                "Returns empty list if no work items found for the assignee"
            );
        });

        Options(x =>
        {
            Debug.Assert(x is not null, "Options configuration object cannot be null.");
            x.WithTags("WorkItems");
        });
    }

    public override async Task HandleAsync(
        GetWorkItemsByAssigneeIdRequest req,
        CancellationToken ct
    )
    {
        ArgumentNullException.ThrowIfNull(req);

        GetWorkItemsByAssigneeIdQuery query = new(req.UserId);

        IEnumerable<WorkItemEntity> workItems = await query.ExecuteAsync(ct).ConfigureAwait(false);

        await Send.OkAsync(Map.FromEntity(workItems), ct).ConfigureAwait(false);
    }
}
