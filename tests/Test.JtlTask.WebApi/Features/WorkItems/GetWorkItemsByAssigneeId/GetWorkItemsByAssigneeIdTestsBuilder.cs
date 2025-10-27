// <copyright file="GetWorkItemsByAssigneeIdTestsBuilder.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;
using Test.JtlTask.WebApi.Features.WorkItems.AssignWorkItem;
using GetWorkItemsByAssigneeIdEndpoint = JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId.Endpoint;

namespace Test.JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;

/// <summary>
/// Builder for constructing GetWorkItemsByAssigneeId test scenarios with a fluent API.
/// </summary>
internal sealed class GetWorkItemsByAssigneeIdTestsBuilder
{
    private readonly App _app;
    private readonly List<(
        Guid userId,
        string title,
        string description,
        string priority,
        int estimatedHours
    )> _workItems = [];
    private Guid _assigneeId;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkItemsByAssigneeIdTestsBuilder"/> class.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    private GetWorkItemsByAssigneeIdTestsBuilder(App app) => _app = app;

    /// <summary>
    /// Creates a new instance of the <see cref="GetWorkItemsByAssigneeIdTestsBuilder"/>.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    /// <returns>A new builder instance.</returns>
    public static GetWorkItemsByAssigneeIdTestsBuilder New(App app)
    {
        Debug.Assert(app is not null, "App fixture must not be null.");
        return new(app);
    }

    /// <summary>
    /// Sets the assignee ID for which to retrieve work items.
    /// </summary>
    /// <param name="assigneeId">The assignee ID.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public GetWorkItemsByAssigneeIdTestsBuilder ForAssignee(Guid assigneeId)
    {
        _assigneeId = assigneeId;
        return this;
    }

    /// <summary>
    /// Adds a work item to be assigned to the specified user.
    /// </summary>
    /// <param name="userId">The user ID to assign the work item to.</param>
    /// <param name="title">The title of the work item.</param>
    /// <param name="description">The description of the work item.</param>
    /// <param name="priority">The priority level of the work item.</param>
    /// <param name="estimatedHours">The estimated hours for the work item.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public GetWorkItemsByAssigneeIdTestsBuilder WithWorkItem(
        Guid userId,
        string title,
        string description,
        string priority,
        int estimatedHours
    )
    {
        Debug.Assert(userId != Guid.Empty, "User ID must not be empty.");
        Debug.Assert(!string.IsNullOrWhiteSpace(title), "Title must not be null or whitespace.");
        Debug.Assert(
            !string.IsNullOrWhiteSpace(description),
            "Description must not be null or whitespace."
        );
        Debug.Assert(
            !string.IsNullOrWhiteSpace(priority),
            "Priority must not be null or whitespace."
        );
        Debug.Assert(estimatedHours > 0, "Estimated hours must be positive.");

        _workItems.Add((userId, title, description, priority, estimatedHours));
        return this;
    }

    /// <summary>
    /// Executes the test scenario by first assigning all work items and then retrieving them for the specified assignee.
    /// </summary>
    /// <returns>The test result containing the response and retrieved work items.</returns>
    public async Task<TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>>> ExecuteAsync()
    {
        // First, assign all work items using AssignWorkItemTestsBuilder
        foreach (
            (
                Guid userId,
                string title,
                string description,
                string priority,
                int estimatedHours
            ) in _workItems
        )
        {
            await AssignWorkItemTestsBuilder
                .New(_app)
                .WithValidWorkItem(userId, title, description, priority, estimatedHours)
                .ExecuteAsync()
                .ConfigureAwait(false);
        }

        // Then retrieve work items for the assignee
        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result = await _app
            .Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(_assigneeId))
            .ConfigureAwait(false);

        Debug.Assert(result is not null, "Result must not be null after execution.");
        return result;
    }

    /// <summary>
    /// Executes the test scenario without assigning any work items, only retrieving for the specified assignee.
    /// </summary>
    /// <returns>The test result containing the response and retrieved work items.</returns>
    public async Task<
        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>>
    > ExecuteWithoutWorkItemsAsync()
    {
        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result = await _app
            .Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(_assigneeId))
            .ConfigureAwait(false);

        Debug.Assert(result is not null, "Result must not be null after execution.");
        return result;
    }
}
