using System.Diagnostics;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using SharedKernel.Abstractions;
using SharedKernel.Models.Common;
using Tasks.Domain.Models.Tasks;
using WorkItems.Domain.Models.WorkItems.ValueObjects;
using static Monads.Results.ResultFactory;

namespace WorkItems.Domain.Models.WorkItems;

/// <summary>
/// Represents a task that can be assigned to a user.
/// </summary>
internal sealed class WorkItem : Entity, IAggregateRoot
{
    /// <summary>
    /// Gets the unique identifier of the user assigned to this task.
    /// </summary>
    public AssigneeId AssigneeId { get; private set; }

    /// <summary>
    /// Gets the title of the task.
    /// </summary>
    public Title Title { get; private set; }

    /// <summary>
    /// Gets the detailed description of the task.
    /// </summary>
    public Description Description { get; private set; }

    /// <summary>
    /// Gets the current status of the task.
    /// </summary>
    public Status Status { get; private set; }

    /// <summary>
    /// Gets the priority level of the task.
    /// </summary>
    public Priority Priority { get; private set; }

    /// <summary>
    /// Gets the due date for the task.
    /// </summary>
    public DueDate? DueDate { get; private set; }

    /// <summary>
    /// Gets the date when the task was completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    /// Gets the estimated effort in hours.
    /// </summary>
    public EstimatedHours EstimatedHours { get; private set; }

    /// <summary>
    /// Gets the tags associated with this task.
    /// </summary>
    public Tags Tags { get; private set; }

    /// <summary>
    /// Gets the identifier of the parent task if this is a subtask.
    /// </summary>
    public Guid ParentTaskId { get; private set; }

    private WorkItem(
        Guid id,
        AssigneeId assigneeId,
        Title title,
        Description description,
        Status status,
        Priority priority,
        DueDate? dueDate,
        DateTimeOffset? completedAt,
        EstimatedHours estimatedHours,
        Tags tags,
        Guid parentTaskId
    )
    {
        Id = id;
        AssigneeId = assigneeId;
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        CompletedAt = completedAt;
        EstimatedHours = estimatedHours;
        Tags = tags;
        ParentTaskId = parentTaskId;
    }

    /// <summary>
    /// Creates a new task instance.
    /// </summary>
    /// <param name="assigneeId">The identifier of the user to assign the task to.</param>
    /// <param name="title">The title of the task.</param>
    /// <param name="description">The detailed description of the task.</param>
    /// <param name="priority">The priority level of the task.</param>
    /// <param name="estimatedHours">The estimated effort in hours.</param>
    /// <param name="dueDate">The optional due date for the task.</param>
    /// <param name="parentTaskId">The optional identifier of the parent task if this is a subtask.</param>
    /// <param name="tags">The optional tags associated with this task.</param>
    /// <param name="timeProvider">The time provider for getting the current time.</param>
    /// <returns>A result containing the created task or a domain error.</returns>
    public static Result<WorkItem, DomainError> Create(
        Guid assigneeId,
        string title,
        string description,
        string priority,
        int estimatedHours,
        DateTimeOffset? dueDate,
        Guid parentTaskId,
        IEnumerable<string> tags,
        TimeProvider timeProvider
    )
    {
        Debug.Assert(timeProvider is not null, "Time provider cannot be null.");

        return AssigneeIdFactory
            .Create(assigneeId)
            .Bind(id =>
                TitleFactory
                    .Create(title)
                    .Bind(title =>
                        DescriptionFactory
                            .Create(description)
                            .Bind(description =>
                                EstimatedHoursFactory
                                    .Create(estimatedHours)
                                    .Bind(estimatedHours =>
                                        DueDateFactory
                                            .Create(dueDate, timeProvider)
                                            .Bind(dueDate =>
                                                priority
                                                    .TryIntoPriority()
                                                    .Map(priority => new WorkItem(
                                                        Guid.NewGuid(),
                                                        id,
                                                        title,
                                                        description,
                                                        Status.Todo,
                                                        priority,
                                                        dueDate,
                                                        null,
                                                        estimatedHours,
                                                        TagsFactory.Create(tags),
                                                        parentTaskId
                                                    ))
                                            )
                                    )
                            )
                    )
            );
    }

    /// <summary>
    /// Assigns the task to a user.
    /// </summary>
    /// <param name="assigneeId">The identifier of the user to assign the task to.</param>
    public Result<Unit, DomainError> AssignTo(Guid assigneeId) =>
        AssigneeIdFactory
            .Create(assigneeId)
            .Map(id =>
            {
                AssigneeId = id;
                return Unit.Default;
            });

    /// <summary>
    /// Updates the task status.
    /// </summary>
    /// <param name="status">The new status.</param>
    /// <param name="timeProvider">The time provider for getting the current time.</param>
    public Result<Unit, DomainError> UpdateStatus(string status, TimeProvider timeProvider) =>
        Status
            .ValidateTransition(status)
            .Map(newStatus =>
            {
                Status = newStatus;

                if (Status.IsTerminal())
                {
                    CompletedAt = timeProvider.GetUtcNow();
                    Debug.Assert(
                        CompletedAt.HasValue,
                        "CompletedAt must have a value for terminal tasks."
                    );
                }

                return Unit.Default;
            });

    /// <summary>
    /// Updates the task priority.
    /// </summary>
    /// <param name="priority">The new priority level.</param>
    public Result<Unit, DomainError> UpdatePriority(string priority) =>
        priority
            .TryIntoPriority()
            .Map(newPriority =>
            {
                Priority = newPriority;
                return Unit.Default;
            });

    /// <summary>
    /// Sets the due date for the task.
    /// </summary>
    /// <param name="dueDate">The due date.</param>
    /// <param name="timeProvider">The time provider for getting the current time.</param>
    public Result<Unit, DomainError> SetDueDate(
        DateTimeOffset dueDate,
        TimeProvider timeProvider
    ) =>
        DueDateFactory
            .Create(dueDate, timeProvider)
            .Map(dueDateValue =>
            {
                DueDate = dueDateValue;
                return Unit.Default;
            });

    /// <summary>
    /// Updates the task details.
    /// </summary>
    /// <param name="title">The task title.</param>
    /// <param name="description">The task description.</param>
    public Result<Unit, DomainError> UpdateDetails(string title, string description = "") =>
        TitleFactory
            .Create(title)
            .Bind(title =>
                DescriptionFactory
                    .Create(description)
                    .Map(description =>
                    {
                        Title = title;
                        Description = description;
                        return Unit.Default;
                    })
            );

    /// <summary>
    /// Rehydrates a <see cref="WorkItem"/> instance from stored values.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="assigneeId">The assignee identifier.</param>
    /// <param name="title">The task title.</param>
    /// <param name="description">The task description.</param>
    /// <param name="status">The task status.</param>
    /// <param name="priority">The task priority.</param>
    /// <param name="dueDate">The due date.</param>
    /// <param name="completedAt">The completion date.</param>
    /// <param name="estimatedHours">The estimated hours.</param>
    /// <param name="tags">The tags.</param>
    /// <param name="parentTaskId">The parent task identifier.</param>
    /// <returns>The rehydrated <see cref="WorkItem"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when any of the stored values are invalid.</exception>
    /// <exception cref="UnreachableException">Thrown when an unexpected value is encountered.</exception>
    internal static WorkItem Rehydrate(
        Guid id,
        Guid assigneeId,
        string title,
        string description,
        string status,
        string priority,
        DateTimeOffset? dueDate,
        DateTimeOffset? completedAt,
        int estimatedHours,
        IEnumerable<string> tags,
        Guid parentTaskId
    ) =>
        new(
            id,
            AssigneeIdFactory.Rehydrate(assigneeId),
            TitleFactory.Rehydrate(title),
            DescriptionFactory.Rehydrate(description),
            status.IntoStatus(),
            priority.IntoPriority(),
            DueDateFactory.Rehydrate(dueDate),
            completedAt,
            EstimatedHoursFactory.Rehydrate(estimatedHours),
            TagsFactory.Create(tags),
            parentTaskId
        );
}
