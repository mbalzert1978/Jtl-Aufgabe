// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using WorkItems.Application.Adapters;

namespace JtlTask.WebApi.Features.Users.GetWorkItemsByAssigneeId;

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
        Get("/{userId:guid}/work-items");
        Group<Route>();

        Description(b =>
        {
            Debug.Assert(b is not null, "Description configuration object cannot be null.");
            b.Produces<IEnumerable<GetWorkItemsByAssigneeIdResponse>>(StatusCodes.Status200OK);
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
