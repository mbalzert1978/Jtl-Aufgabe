// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Net;
using FastEndpoints;
using Mediator;
using Monads.Results;
using Monads.Results.Extensions.Async;
using Monads.Results.Extensions.Sync;
using SharedKernel.Abstractions;
using WorkItems.Application.Adapters;
using WorkItems.Application.UseCases.AssignWorkItem;

namespace JtlTask.WebApi.Features.WorkItems.AssignWorkItem;

/// <summary>
/// Endpoint for assigning a new work item to a user in the system.
/// </summary>
internal sealed class Endpoint
    : Endpoint<AssignWorkItemRequest, AssignWorkItemResponse, AssignWorkItemMapper>
{
    private const string InternalServerError = "An internal server error occurred.";
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
        Post("api/v1/users/{userId:guid}/workitems");

        Summary(s =>
        {
            Debug.Assert(s is not null, "Summary configuration object cannot be null.");

            s.Summary = "Assign a new work item to a user";
            s.Description = "Creates a new work item and assigns it to the specified user";
            s.Response<AssignWorkItemResponse>(
                StatusCodes.Status201Created,
                "The work item was successfully assigned."
            );
            s.Response<ProblemDetails>(StatusCodes.Status400BadRequest, "The request was invalid.");
            s.Response<ProblemDetails>(
                StatusCodes.Status404NotFound,
                "The specified user was not found."
            );
            s.Response<InternalErrorResponse>(
                StatusCodes.Status500InternalServerError,
                "An internal server error occurred."
            );
        });

        Description(b =>
            b.Produces<AssignWorkItemResponse>(StatusCodes.Status201Created, "application/json")
                .ProducesProblemDetails(StatusCodes.Status400BadRequest)
                .ProducesProblemDetails(StatusCodes.Status404NotFound)
                .ProducesProblemFE<InternalErrorResponse>(StatusCodes.Status500InternalServerError)
        );

        Options(x =>
        {
            Debug.Assert(x is not null, "Options configuration object cannot be null.");
            x.WithTags("WorkItems");
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
            await _mediator.SendAsync<AssignWorkItemCommand, WorkItemEntity>(cmd, ct);

        await result
            .Map(Map.FromEntity)
            .Match(
                response => SendSuccessResponseAsync(response, ct),
                error => HandleErrorAsync(error, ct)
            );
    }

    /// <summary>
    /// Sends a successful response with the assigned work item data.
    /// </summary>
    /// <param name="response">The response containing the assigned work item data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SendSuccessResponseAsync(
        AssignWorkItemResponse response,
        CancellationToken ct
    )
    {
        Debug.Assert(response is not null, "Response should not be null.");
        Debug.Assert(response.Id != Guid.Empty, "Response ID must be valid.");
        Debug.Assert(response.AssigneeId != Guid.Empty, "Response AssigneeId must be valid.");
        Debug.Assert(
            !string.IsNullOrWhiteSpace(response.Title),
            "Response title should not be empty."
        );

        await PublishAsync(
            new WorkItemAssignedEvent(response.Id, response.AssigneeId),
            cancellation: ct
        );
        await Send.ResponseAsync(response, StatusCodes.Status201Created, ct);
    }

    /// <summary>
    /// Handles errors by mapping them to appropriate HTTP responses.
    /// </summary>
    /// <param name="error">The error to handle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleErrorAsync(IError error, CancellationToken ct)
    {
        Debug.Assert(error is not null, "Error cannot be null.");
        Debug.Assert(!string.IsNullOrWhiteSpace(error.Message), "Error message cannot be empty.");

        switch (error.ErrorType)
        {
            case ErrorType.Validation:
                AddError(error.Message);
                await Send.ErrorsAsync(StatusCodes.Status400BadRequest, ct);
                break;

            case ErrorType.NotFound:
                AddError(error.Message);
                await Send.ErrorsAsync(StatusCodes.Status404NotFound, ct);
                break;

            default:
                AddError(InternalServerError);
                await Send.ErrorsAsync(StatusCodes.Status500InternalServerError, ct);
                break;
        }
    }
}
