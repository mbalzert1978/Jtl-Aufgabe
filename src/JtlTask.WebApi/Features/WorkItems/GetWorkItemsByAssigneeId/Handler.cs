// <copyright file="Handler.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using WorkItems.Application.Adapters;
using WorkItems.Infrastructure.Persistence;

namespace JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;

/// <summary>
/// Handles the execution of get work items by assignee ID query by delegating to the application layer.
/// </summary>
internal sealed class GetWorkItemsByAssigneeIdHandler
    : ICommandHandler<GetWorkItemsByAssigneeIdQuery, IEnumerable<WorkItemEntity>>
{
    private readonly WorkItemsDbContext _context;

    public GetWorkItemsByAssigneeIdHandler(WorkItemsDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        Debug.Assert(_context == context, "DbContext must be initialized.");
    }

    public async Task<IEnumerable<WorkItemEntity>> ExecuteAsync(
        GetWorkItemsByAssigneeIdQuery command,
        CancellationToken ct
    )
    {
        ArgumentNullException.ThrowIfNull(command);

        List<WorkItemEntity> workItems = await _context
            .WorkItems.Include(w => w.Tags)
            .Where(w => w.AssigneeId == command.AssigneeId)
            .ToListAsync(ct);

        Debug.Assert(workItems is not null, "Query result must not be null, even if empty.");
        Debug.Assert(
            workItems.All(w => w.AssigneeId == command.AssigneeId),
            "All retrieved work items must match the requested AssigneeId."
        );

        return workItems;
    }
}
