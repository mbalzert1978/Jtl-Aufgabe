namespace WorkItems.Domain.Models.WorkItems.ValueObjects;

/// <summary>
/// Represents the priority level of a task.
/// </summary>
internal enum Priority
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
