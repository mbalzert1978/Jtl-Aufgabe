namespace WorkItems.Domain.Models.WorkItems.ValueObjects;

/// <summary>
/// Represents the status of a task.
/// </summary>
internal enum Status
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
