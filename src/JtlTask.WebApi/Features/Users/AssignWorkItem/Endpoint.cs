// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Mediator;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using SharedKernel.Abstractions;
using WorkItems.Application.Adapters;
using WorkItems.Application.UseCases.AssignWorkItem;

namespace JtlTask.WebApi.Features.Users.AssignWorkItem;

/// <summary>
/// Endpoint for assigning a new work item to a user in the system.
/// </summary>
internal sealed class Endpoint
    : Endpoint<AssignWorkItemRequest, AssignWorkItemResponse, AssignWorkItemMapper>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        _mediator = mediator;

        Debug.Assert(_mediator == mediator, "Mediator assignment must be successful.");
    }

    /// <summary>
    /// Configures the endpoint routing, security, and documentation.
    /// </summary>
    public override void Configure()
    {
        Debug.Assert(Config is not null, "Config must be initialized.");

        AllowAnonymous();
        Post("/{userId:guid}/work-items");
        Group<Route>();

        Description(b =>
        {
            Debug.Assert(b is not null, "Description configuration object cannot be null.");
            b.Produces<AssignWorkItemResponse>(StatusCodes.Status201Created);
            b.ProducesProblemDetails(StatusCodes.Status400BadRequest);
            b.ProducesProblemDetails(StatusCodes.Status404NotFound);
        });
    }

    /// <summary>
    /// Handles the work item assignment request.
    /// </summary>
    /// <param name="req">The assignment request containing work item data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(AssignWorkItemRequest req, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(req);

        AssignWorkItemCommand cmd = new(
            req.UserId,
            req.Title,
            req.EstimatedHours,
            req.ParentTaskId ?? Guid.Empty,
            req.Description,
            req.Priority,
            req.DueDate,
            req.Tags
        );

        Debug.Assert(cmd.AssigneeId == req.UserId, "Command AssigneeId must match request.");
        Debug.Assert(cmd.Title == req.Title, "Command Title must match request.");
        Debug.Assert(
            cmd.EstimatedHours == req.EstimatedHours,
            "Command EstimatedHours must match request."
        );
        // csharpier-ignore
        Result<WorkItemEntity, IError> result =
            await _mediator.SendAsync<AssignWorkItemCommand, WorkItemEntity>(cmd, ct).ConfigureAwait(false);

        await result
            .Map(Map.FromEntity)
            .Match(
                response => Send.ResponseAsync(response, StatusCodes.Status201Created, ct),
                error =>
                {
                    switch (error.ErrorType)
                    {
                        case ErrorType.Validation:
                            AddError(error.Message);
                            Send.ErrorsAsync(StatusCodes.Status400BadRequest, ct);
                            break;

                        case ErrorType.NotFound:
                            AddError(error.Message);
                            Send.ErrorsAsync(StatusCodes.Status404NotFound, ct);
                            break;

                        default:
                            Send.ErrorsAsync(StatusCodes.Status500InternalServerError, ct);
                            break;
                    }

                    return Task.CompletedTask;
                }
            )
            .ConfigureAwait(false);
    }
}
