// <copyright file="AssignWorkItemTestsBuilder.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using JtlTask.WebApi.Features.Users.AssignWorkItem;

namespace Test.JtlTask.WebApi.Features.Users.AssignWorkItem;

/// <summary>
/// Builder for constructing AssignWorkItem test scenarios with a fluent API.
/// </summary>
internal sealed class AssignWorkItemTestsBuilder
{
    private readonly App _app;
    private Task<TestResult<AssignWorkItemResponse>>? _assignTask;
    private Task<TestResult<ProblemDetails>>? _problemTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="AssignWorkItemTestsBuilder"/> class.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    private AssignWorkItemTestsBuilder(App app) => _app = app;

    /// <summary>
    /// Creates a new instance of the <see cref="AssignWorkItemTestsBuilder"/>.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    /// <returns>A new builder instance.</returns>
    public static AssignWorkItemTestsBuilder New(App app)
    {
        Debug.Assert(app is not null, "App fixture must not be null.");
        return new(app);
    }

    /// <summary>
    /// Configures the builder to assign a work item with valid data.
    /// </summary>
    /// <param name="userId">The user ID to assign the work item to.</param>
    /// <param name="title">The title of the work item.</param>
    /// <param name="description">The description of the work item.</param>
    /// <param name="priority">The priority level of the work item.</param>
    /// <param name="estimatedHours">The estimated hours for the work item.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public AssignWorkItemTestsBuilder WithValidWorkItem(
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

        _assignTask = _app.Client.POSTAsync<
            Endpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: userId,
                Title: title,
                Description: description,
                Priority: priority,
                EstimatedHours: estimatedHours
            )
        );
        Debug.Assert(_assignTask is not null, "Assign task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to assign a work item with tags.
    /// </summary>
    /// <param name="userId">The user ID to assign the work item to.</param>
    /// <param name="title">The title of the work item.</param>
    /// <param name="description">The description of the work item.</param>
    /// <param name="priority">The priority level of the work item.</param>
    /// <param name="estimatedHours">The estimated hours for the work item.</param>
    /// <param name="tags">The tags to associate with the work item.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public AssignWorkItemTestsBuilder WithTags(
        Guid userId,
        string title,
        string description,
        string priority,
        int estimatedHours,
        IEnumerable<string> tags
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
        Debug.Assert(tags is not null, "Tags must not be null.");

        _assignTask = _app.Client.POSTAsync<
            Endpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: userId,
                Title: title,
                Description: description,
                Priority: priority,
                EstimatedHours: estimatedHours,
                Tags: tags
            )
        );
        Debug.Assert(_assignTask is not null, "Assign task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to assign a work item with a due date.
    /// </summary>
    /// <param name="userId">The user ID to assign the work item to.</param>
    /// <param name="title">The title of the work item.</param>
    /// <param name="description">The description of the work item.</param>
    /// <param name="priority">The priority level of the work item.</param>
    /// <param name="estimatedHours">The estimated hours for the work item.</param>
    /// <param name="dueDate">The due date for the work item.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public AssignWorkItemTestsBuilder WithDueDate(
        Guid userId,
        string title,
        string description,
        string priority,
        int estimatedHours,
        DateTimeOffset dueDate
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

        _assignTask = _app.Client.POSTAsync<
            Endpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: userId,
                Title: title,
                Description: description,
                Priority: priority,
                EstimatedHours: estimatedHours,
                DueDate: dueDate
            )
        );
        Debug.Assert(_assignTask is not null, "Assign task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to assign a work item with an empty title for testing validation.
    /// </summary>
    /// <param name="userId">The user ID to assign the work item to.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public AssignWorkItemTestsBuilder WithEmptyTitle(Guid userId)
    {
        Debug.Assert(userId != Guid.Empty, "User ID must not be empty.");

        _problemTask = _app.Client.POSTAsync<Endpoint, AssignWorkItemRequest, ProblemDetails>(
            new(
                UserId: userId,
                Title: string.Empty,
                Description: "Test description",
                Priority: "High",
                EstimatedHours: 5
            )
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to assign a work item with a title below the minimum length for testing validation.
    /// </summary>
    /// <param name="userId">The user ID to assign the work item to.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public AssignWorkItemTestsBuilder WithTitleTooShort(Guid userId)
    {
        Debug.Assert(userId != Guid.Empty, "User ID must not be empty.");

        _problemTask = _app.Client.POSTAsync<Endpoint, AssignWorkItemRequest, ProblemDetails>(
            new(
                UserId: userId,
                Title: "ab",
                Description: "Test description",
                Priority: "High",
                EstimatedHours: 5
            )
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to assign a work item with an empty description for testing validation.
    /// </summary>
    /// <param name="userId">The user ID to assign the work item to.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public AssignWorkItemTestsBuilder WithEmptyDescription(Guid userId)
    {
        Debug.Assert(userId != Guid.Empty, "User ID must not be empty.");

        _problemTask = _app.Client.POSTAsync<Endpoint, AssignWorkItemRequest, ProblemDetails>(
            new(
                UserId: userId,
                Title: "Valid Title",
                Description: string.Empty,
                Priority: "High",
                EstimatedHours: 5
            )
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to assign a work item with an empty user ID for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public AssignWorkItemTestsBuilder WithEmptyUserId()
    {
        _problemTask = _app.Client.POSTAsync<Endpoint, AssignWorkItemRequest, ProblemDetails>(
            new(
                UserId: Guid.Empty,
                Title: "Valid Title",
                Description: "Valid description",
                Priority: "High",
                EstimatedHours: 5
            )
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Executes the assign work item request expecting a successful response.
    /// </summary>
    /// <returns>The work item assignment response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the assign task has not been initialized.</exception>
    public async Task<TestResult<AssignWorkItemResponse>> ExecuteAsync()
    {
        if (_assignTask is null)
            throw new InvalidOperationException(
                "Assign task has not been initialized. Call a With* method before executing."
            );

        TestResult<AssignWorkItemResponse> result = await _assignTask.ConfigureAwait(false);
        Debug.Assert(result is not null, "Result must not be null after execution.");
        return result;
    }

    /// <summary>
    /// Executes the assign work item request expecting a validation error response.
    /// </summary>
    /// <returns>The problem details response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the problem task has not been initialized.</exception>
    public async Task<TestResult<ProblemDetails>> ExecuteProblemAsync()
    {
        if (_problemTask is null)
            throw new InvalidOperationException(
                "Problem task has not been initialized. Call a With* method before executing."
            );

        TestResult<ProblemDetails> result = await _problemTask.ConfigureAwait(false);
        Debug.Assert(result is not null, "Result must not be null after execution.");
        return result;
    }
}
