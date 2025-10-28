// <copyright file="Models.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using FluentValidation;

namespace JtlTask.WebApi.Features.Users.AssignWorkItem;

/// <summary>
/// Represents a request to assign a new work item to a user.
/// </summary>
/// <param name="UserId">The unique identifier of the user to assign the work item to.</param>
/// <param name="Title">The title of the work item.</param>
/// <param name="Description">The detailed description of the work item.</param>
/// <param name="Priority">The priority level of the work item.</param>
/// <param name="EstimatedHours">The estimated effort in hours.</param>
/// <param name="DueDate">The optional due date for the work item.</param>
/// <param name="ParentTaskId">The optional identifier of the parent task if this is a subtask.</param>
/// <param name="Tags">The optional collection of tags associated with the work item.</param>
internal sealed record AssignWorkItemRequest(
    Guid UserId,
    string Title,
    string Description,
    string Priority,
    int EstimatedHours,
    DateTimeOffset? DueDate = null,
    Guid? ParentTaskId = null,
    IEnumerable<string>? Tags = null
);

/// <summary>
/// Represents the response for a successfully assigned work item.
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
internal sealed record AssignWorkItemResponse(
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

/// <summary>
/// Event published when a work item is successfully assigned.
/// </summary>
/// <param name="WorkItemId">The unique identifier of the assigned work item.</param>
/// <param name="AssigneeId">The unique identifier of the user assigned to the work item.</param>
internal sealed record WorkItemAssignedEvent(Guid WorkItemId, Guid AssigneeId) : IEvent;

/// <summary>
/// Validator for <see cref="AssignWorkItemRequest"/>.
/// </summary>
internal sealed class AssignWorkItemRequestValidator : Validator<AssignWorkItemRequest>
{
    private const int MinTitleLength = 3;
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 4096;
    private const int MaxPriorityLength = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignWorkItemRequestValidator"/> class.
    /// </summary>
    public AssignWorkItemRequestValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("AssigneeId is required.")
            .NotEqual(Guid.Empty)
            .WithMessage("AssigneeId must be a valid GUID.");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MinimumLength(MinTitleLength)
            .WithMessage($"Title must be at least {MinTitleLength} characters long.")
            .MaximumLength(MaxTitleLength)
            .WithMessage($"Title must be less than {MaxTitleLength} characters long.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(MaxDescriptionLength)
            .WithMessage($"Description must be less than {MaxDescriptionLength} characters long.");

        RuleFor(x => x.Priority)
            .NotEmpty()
            .WithMessage("Priority is required.")
            .MaximumLength(MaxPriorityLength)
            .WithMessage($"Priority must be less than {MaxPriorityLength} characters long.");

        RuleFor(x => x.EstimatedHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("EstimatedHours must not be negative.");

        RuleFor(x => x.DueDate)
            .GreaterThan(timeProvider.GetUtcNow())
            .When(x => x.DueDate.HasValue)
            .WithMessage("DueDate must be in the future.");
    }
}
