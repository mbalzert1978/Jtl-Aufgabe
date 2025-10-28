// <copyright file="AssignWorkItemTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.Users.AssignWorkItem;

namespace Test.JtlTask.WebApi.Features.Users.AssignWorkItem;

/// <summary>
/// End-to-end tests for the AssignWorkItem endpoint.
/// </summary>
public class AssignWorkItemTests(App app) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task AssignWorkItem_WhenTitleIsEmpty_ShouldReturnBadRequest()
    {
        TestUser user = await app.CreateTestUser();

        AssignWorkItemTestsBuilder builder = AssignWorkItemTestsBuilder
            .New(app)
            .WithEmptyTitle(user.UserId);

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "title").ShouldBeTrue();

        await app.CleanDatabaseAsync();
    }

    [Fact, Priority(2)]
    public async Task AssignWorkItem_WhenTitleTooShort_ShouldReturnBadRequest()
    {
        TestUser user = await app.CreateTestUser();

        AssignWorkItemTestsBuilder builder = AssignWorkItemTestsBuilder
            .New(app)
            .WithTitleTooShort(user.UserId);

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "title").ShouldBeTrue();

        await app.CleanDatabaseAsync();
    }

    [Fact, Priority(3)]
    public async Task AssignWorkItem_WhenDescriptionIsEmpty_ShouldReturnBadRequest()
    {
        TestUser user = await app.CreateTestUser();

        AssignWorkItemTestsBuilder builder = AssignWorkItemTestsBuilder
            .New(app)
            .WithEmptyDescription(user.UserId);

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "description").ShouldBeTrue();

        await app.CleanDatabaseAsync();
    }

    [Fact, Priority(4)]
    public async Task AssignWorkItem_WhenUserIdIsEmpty_ShouldReturnBadRequest()
    {
        AssignWorkItemTestsBuilder builder = AssignWorkItemTestsBuilder.New(app).WithEmptyUserId();

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "userId").ShouldBeTrue();
    }

    [Fact, Priority(5)]
    public async Task AssignWorkItem_WhenValidRequest_ShouldReturnCreatedWithWorkItemData()
    {
        string title = "Implement feature X";
        string description = "This is a detailed description of the work item";
        string priority = "High";
        int estimatedHours = 8;
        TestUser user = await app.CreateTestUser();

        AssignWorkItemTestsBuilder builder = AssignWorkItemTestsBuilder
            .New(app)
            .WithValidWorkItem(user.UserId, title, description, priority, estimatedHours);

        TestResult<AssignWorkItemResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.Id.ShouldNotBe(Guid.Empty);
        result.Result.AssigneeId.ShouldBe(user.UserId);
        result.Result.Title.ShouldBe(title);
        result.Result.Description.ShouldBe(description);
        result.Result.Priority.ShouldBe(priority);
        result.Result.EstimatedHours.ShouldBe(estimatedHours);
        result.Result.Status.ShouldNotBeNullOrEmpty();

        await app.CleanDatabaseAsync();
    }

    [Fact, Priority(6)]
    public async Task AssignWorkItem_WhenValidRequestWithTags_ShouldReturnCreatedWithTags()
    {
        string[] tags = ["backend", "api", "urgent"];
        TestUser user = await app.CreateTestUser();

        AssignWorkItemTestsBuilder builder = AssignWorkItemTestsBuilder
            .New(app)
            .WithTags(
                user.UserId,
                "Implement API endpoint",
                "Create a new REST API endpoint",
                "High",
                5,
                tags
            );

        TestResult<AssignWorkItemResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.Tags.ShouldNotBeNull();
        result.Result.Tags.Count().ShouldBe(3);
        result.Result.Tags.ShouldContain("backend");
        result.Result.Tags.ShouldContain("api");
        result.Result.Tags.ShouldContain("urgent");

        await app.CleanDatabaseAsync();
    }

    [Fact, Priority(7)]
    public async Task AssignWorkItem_WhenValidRequestWithDueDate_ShouldReturnCreatedWithDueDate()
    {
        TestUser user = await app.CreateTestUser();
        // Test time is fixed at October 27, 2025, 12:00:00 UTC, so use November 3, 2025
        DateTimeOffset dueDate = new(2025, 11, 3, 12, 0, 0, TimeSpan.Zero);

        AssignWorkItemTestsBuilder builder = AssignWorkItemTestsBuilder
            .New(app)
            .WithDueDate(
                user.UserId,
                "Complete documentation",
                "Write comprehensive documentation",
                "Normal",
                10,
                dueDate
            );

        TestResult<AssignWorkItemResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.DueDate.ShouldNotBeNull();
        result.Result.DueDate.Value.Date.ShouldBe(dueDate.Date);

        await app.CleanDatabaseAsync();
    }

    [Fact, Priority(8)]
    public async Task AssignWorkItem_WhenMultipleWorkItemsAssigned_ShouldCreateMultipleItems()
    {
        TestUser user = await app.CreateTestUser();
        AssignWorkItemTestsBuilder builder1 = AssignWorkItemTestsBuilder
            .New(app)
            .WithValidWorkItem(
                user.UserId,
                "First work item",
                "Description of first item",
                "High",
                5
            );

        AssignWorkItemTestsBuilder builder2 = AssignWorkItemTestsBuilder
            .New(app)
            .WithValidWorkItem(
                user.UserId,
                "Second work item",
                "Description of second item",
                "Low",
                3
            );

        TestResult<AssignWorkItemResponse> result1 = await builder1.ExecuteAsync();
        TestResult<AssignWorkItemResponse> result2 = await builder2.ExecuteAsync();

        result1.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result2.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result1.Result.ShouldNotBeNull();
        result2.Result.ShouldNotBeNull();
        result1.Result.Id.ShouldNotBe(result2.Result.Id);
        result1.Result.AssigneeId.ShouldBe(user.UserId);
        result2.Result.AssigneeId.ShouldBe(user.UserId);

        await app.CleanDatabaseAsync();
    }
}
