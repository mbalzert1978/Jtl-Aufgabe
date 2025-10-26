// <copyright file="TaskTags.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks.ValueObjects;

/// <summary>
/// Represents an immutable collection of tags associated with a task.
/// </summary>
internal sealed record TaskTags(ImmutableList<string> Value);

/// <summary>
/// Factory for creating <see cref="TaskTags"/> instances.
/// </summary>
internal static class TaskTagsFactory
{
    /// <summary>
    /// Creates a new <see cref="TaskTags"/> instance.
    /// </summary>
    /// <param name="tags">The collection of tags. Can be null or empty.</param>
    /// <returns>A result containing the task tags or a domain error.</returns>
    public static TaskTags Create(IEnumerable<string> tags)
    {
        IEnumerable<string> cleanedTags = tags.Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim());
        Debug.Assert(
            cleanedTags.All(t => !string.IsNullOrWhiteSpace(t)),
            "All tags must be non-empty after trimming."
        );

        return new TaskTags([.. cleanedTags]);
    }
}
