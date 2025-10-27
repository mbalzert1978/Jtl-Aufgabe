// <copyright file="TaskEntity.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace WorkItems.Application.Adapters;

/// <summary>
/// Database model representing a tag associated with a work item.
/// </summary>
public sealed class WorkItemTag
{
    /// <summary>
    /// Gets or sets the tag name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemTag"/> class.
    /// Required by EF Core for materialization.
    /// </summary>
    private WorkItemTag() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkItemTag"/> class.
    /// </summary>
    /// <param name="name">The tag name.</param>
    public WorkItemTag(string name)
    {
        Debug.Assert(
            !string.IsNullOrWhiteSpace(name),
            $"{nameof(name)} must not be null or whitespace."
        );
        Name = name;
    }
}
