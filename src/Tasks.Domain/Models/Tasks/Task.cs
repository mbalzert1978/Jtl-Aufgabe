using System.Diagnostics;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using SharedKernel.Abstractions;
using SharedKernel.Models.Common;
using Tasks.Domain.Models.Tasks.ValueObjects;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks;

/// <summary>
/// Represents a task that can be assigned to a user.
/// </summary>
internal sealed class Task : Entity, IAggregateRoot
{
    /// <summary>
    /// Gets the unique identifier of the user assigned to this task.
    /// </summary>
    public AssigneeId AssigneeId { get; private set; }

    /// <summary>
    /// Gets the title of the task.
    /// </summary>
    public TaskTitle Title { get; private set; }

    /// <summary>
    /// Gets the detailed description of the task.
    /// </summary>
    public TaskDescription Description { get; private set; }

    /// <summary>
    /// Gets the current status of the task.
    /// </summary>
    public TaskStatus Status { get; private set; }

    /// <summary>
    /// Gets the priority level of the task.
    /// </summary>
    public TaskPriority Priority { get; private set; }

    /// <summary>
    /// Gets the due date for the task.
    /// </summary>
    public TaskDueDate? DueDate { get; private set; }

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
    public TaskTags Tags { get; private set; }

    /// <summary>
    /// Gets the identifier of the parent task if this is a subtask.
    /// </summary>
    public Guid ParentTaskId { get; private set; }

    private Task(
        Guid id,
        AssigneeId assigneeId,
        TaskTitle title,
        TaskDescription description,
        TaskStatus status,
        TaskPriority priority,
        TaskDueDate? dueDate,
        DateTimeOffset? completedAt,
        EstimatedHours estimatedHours,
        TaskTags tags,
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
    public static Result<Task, DomainError> Create(
        Guid assigneeId,
        string title,
        string description,
        TaskPriority priority,
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
                TaskTitleFactory
                    .Create(title)
                    .Bind(title =>
                        TaskDescriptionFactory
                            .Create(description)
                            .Bind(description =>
                                EstimatedHoursFactory
                                    .Create(estimatedHours)
                                    .Bind(estimatedHours =>
                                        TaskDueDateFactory
                                            .Create(dueDate, timeProvider)
                                            .Map(dueDate =>
                                            {
                                                Task task = new(
                                                    Guid.NewGuid(),
                                                    id,
                                                    title,
                                                    description,
                                                    TaskStatus.Todo,
                                                    priority,
                                                    dueDate,
                                                    null,
                                                    estimatedHours,
                                                    TaskTagsFactory.Create(tags),
                                                    parentTaskId
                                                );
                                                TaskCreatedEvent @event = new(
                                                    task.Id,
                                                    timeProvider.GetUtcNow()
                                                );
                                                task.Raise(@event);
                                                return task;
                                            })
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
    public Result<Unit, DomainError> UpdateStatus(TaskStatus status, TimeProvider timeProvider) =>
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
    public Result<Unit, DomainError> UpdatePriority(TaskPriority priority) =>
        priority
            .Validate()
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
        TaskDueDateFactory
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
        TaskTitleFactory
            .Create(title)
            .Bind(title =>
                TaskDescriptionFactory
                    .Create(description)
                    .Map(description =>
                    {
                        Title = title;
                        Description = description;
                        return Unit.Default;
                    })
            );
}

/// <summary>
/// Represents the status of a task.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task is planned but not yet started.
    /// </summary>
    Todo,

    /// <summary>
    /// Task is currently being worked on.
    /// </summary>
    InProgress,

    /// <summary>
    /// Task is blocked by external dependencies.
    /// </summary>
    Blocked,

    /// <summary>
    /// Task is ready for review.
    /// </summary>
    InReview,

    /// <summary>
    /// Task has been completed.
    /// </summary>
    Completed,

    /// <summary>
    /// Task has been cancelled.
    /// </summary>
    Cancelled,
}

/// <summary>
/// Represents the priority level of a task.
/// </summary>
public enum TaskPriority
{
    /// <summary>
    /// Low priority task.
    /// </summary>
    Low,

    /// <summary>
    /// Normal priority task.
    /// </summary>
    Normal,

    /// <summary>
    /// High priority task.
    /// </summary>
    High,

    /// <summary>
    /// Critical priority task requiring immediate attention.
    /// </summary>
    Critical,
}
