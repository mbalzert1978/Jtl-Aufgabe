// <copyright file="AssignWorkItemTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.User.RegisterUser;
using JtlTask.WebApi.Features.WorkItems.AssignWorkItem;
using AssignWorkItemEndpoint = JtlTask.WebApi.Features.WorkItems.AssignWorkItem.Endpoint;

namespace Test.JtlTask.WebApi.Features.WorkItems.AssignWorkItem;

/// <summary>
/// End-to-end tests for the AssignWorkItem endpoint.
/// </summary>
public class AssignWorkItemTests(App app) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task AssignWorkItem_WhenTitleIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act: Try to assign work item with empty title
        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            ProblemDetails
        >(
            new(
                UserId: userId,
                Title: string.Empty,
                Description: "Test description",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "title").ShouldBeTrue();
    }

    [Fact, Priority(2)]
    public async Task AssignWorkItem_WhenTitleTooShort_ShouldReturnBadRequest()
    {
        var userId = Guid.NewGuid();

        // Act: Try to assign work item with too short title
        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            ProblemDetails
        >(
            new(
                UserId: userId,
                Title: "ab",
                Description: "Test description",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "title").ShouldBeTrue();
    }

    [Fact, Priority(3)]
    public async Task AssignWorkItem_WhenDescriptionIsEmpty_ShouldReturnBadRequest()
    {
        // Arrange: Create a user first
        var userId = Guid.NewGuid();

        // Act: Try to assign work item with empty description
        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            ProblemDetails
        >(
            new(
                UserId: userId,
                Title: "Valid Title",
                Description: string.Empty,
                Priority: "High",
                EstimatedHours: 5
            )
        );

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "description").ShouldBeTrue();
    }

    [Fact, Priority(4)]
    public async Task AssignWorkItem_WhenUserIdIsEmpty_ShouldReturnBadRequest()
    {
        // Act: Try to assign work item with empty user ID
        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            ProblemDetails
        >(
            new(
                UserId: Guid.Empty,
                Title: "Valid Title",
                Description: "Valid description",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "userId").ShouldBeTrue();
    }

    [Fact, Priority(5)]
    public async Task AssignWorkItem_WhenValidRequest_ShouldReturnCreatedWithWorkItemData()
    {
        // Arrange: Create a user first
        var userId = Guid.NewGuid();

        // Act: Assign work item
        string title = "Implement feature X";
        string description = "This is a detailed description of the work item";
        string priority = "High";
        int estimatedHours = 8;

        (HttpResponseMessage? rsp, AssignWorkItemResponse? res) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
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

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.Id.ShouldNotBe(Guid.Empty);
        res.AssigneeId.ShouldBe(userId);
        res.Title.ShouldBe(title);
        res.Description.ShouldBe(description);
        res.Priority.ShouldBe(priority);
        res.EstimatedHours.ShouldBe(estimatedHours);
        res.Status.ShouldNotBeNullOrEmpty();
    }

    [Fact, Priority(6)]
    public async Task AssignWorkItem_WhenValidRequestWithTags_ShouldReturnCreatedWithTags()
    {
        // Arrange: Create a user first
        var userId = Guid.NewGuid();

        // Act: Assign work item with tags
        string[] tags = ["backend", "api", "urgent"];

        (HttpResponseMessage? rsp, AssignWorkItemResponse? res) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: userId,
                Title: "Implement API endpoint",
                Description: "Create a new REST API endpoint",
                Priority: "High",
                EstimatedHours: 5,
                Tags: tags
            )
        );

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.Tags.ShouldNotBeNull();
        res.Tags.Count().ShouldBe(3);
        res.Tags.ShouldContain("backend");
        res.Tags.ShouldContain("api");
        res.Tags.ShouldContain("urgent");
    }

    [Fact, Priority(7)]
    public async Task AssignWorkItem_WhenValidRequestWithDueDate_ShouldReturnCreatedWithDueDate()
    {
        var userId = Guid.NewGuid();

        // Act: Assign work item with due date
        // Test time is fixed at October 27, 2025, 12:00:00 UTC, so use November 3, 2025
        DateTimeOffset dueDate = new(2025, 11, 3, 12, 0, 0, TimeSpan.Zero);

        (HttpResponseMessage? rsp, AssignWorkItemResponse? res) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: userId,
                Title: "Complete documentation",
                Description: "Write comprehensive documentation",
                Priority: "Normal",
                EstimatedHours: 10,
                DueDate: dueDate
            )
        );

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.DueDate.ShouldNotBeNull();
        res.DueDate.Value.Date.ShouldBe(dueDate.Date);
    }

    [Fact, Priority(8)]
    public async Task AssignWorkItem_WhenMultipleWorkItemsAssigned_ShouldCreateMultipleItems()
    {
        // Arrange: Create a user first
        var userId = Guid.NewGuid();

        // Act: Assign multiple work items
        (HttpResponseMessage? rsp1, AssignWorkItemResponse? res1) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: userId,
                Title: "First work item",
                Description: "Description of first item",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        (HttpResponseMessage? rsp2, AssignWorkItemResponse? res2) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: userId,
                Title: "Second work item",
                Description: "Description of second item",
                Priority: "Low",
                EstimatedHours: 3
            )
        );

        // Assert
        rsp1.StatusCode.ShouldBe(HttpStatusCode.Created);
        rsp2.StatusCode.ShouldBe(HttpStatusCode.Created);
        res1.ShouldNotBeNull();
        res2.ShouldNotBeNull();
        res1.Id.ShouldNotBe(res2.Id);
        res1.AssigneeId.ShouldBe(userId);
        res2.AssigneeId.ShouldBe(userId);
    }
}
