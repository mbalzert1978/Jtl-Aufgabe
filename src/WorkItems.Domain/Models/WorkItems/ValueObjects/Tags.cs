// <copyright file="TaskTags.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace WorkItems.Domain.Models.WorkItems.ValueObjects;

/// <summary>
/// Represents an immutable collection of tags associated with a task.
/// </summary>
internal sealed record Tags(ImmutableList<string> Value);

/// <summary>
/// Factory for creating <see cref="Tags"/> instances.
/// </summary>
internal static class TagsFactory
{
    /// <summary>
    /// Creates a new <see cref="Tags"/> instance.
    /// </summary>
    /// <param name="tags">The collection of tags. Can be null or empty.</param>
    /// <returns>A result containing the task tags or a domain error.</returns>
    public static Tags Create(IEnumerable<string> tags)
    {
        IEnumerable<string> cleanedTags = tags.Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim());
        Debug.Assert(
            cleanedTags.All(t => !string.IsNullOrWhiteSpace(t)),
            "All tags must be non-empty after trimming."
        );

        return new Tags([.. cleanedTags]);
    }
}
