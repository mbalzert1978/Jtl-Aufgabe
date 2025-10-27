// <copyright file="GetWorkItemsByAssigneeIdTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;

namespace Test.JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;

/// <summary>
/// End-to-end tests for the GetWorkItemsByAssigneeId endpoint.
/// </summary>
public class GetWorkItemsByAssigneeIdTests(App app) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task GetWorkItemsByAssigneeId_WhenNoWorkItemsExist_ShouldReturnEmptyList()
    {
        var nonExistentUserId = Guid.NewGuid();

        GetWorkItemsByAssigneeIdTestsBuilder builder = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(nonExistentUserId);

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result =
            await builder.ExecuteWithoutWorkItemsAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.ShouldBeEmpty();
    }

    [Fact, Priority(2)]
    public async Task GetWorkItemsByAssigneeId_WhenUserHasNoWorkItems_ShouldReturnEmptyList()
    {
        var assigneeId = Guid.NewGuid();

        GetWorkItemsByAssigneeIdTestsBuilder builder = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(assigneeId);

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result =
            await builder.ExecuteWithoutWorkItemsAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.ShouldBeEmpty();
    }

    [Fact, Priority(3)]
    public async Task GetWorkItemsByAssigneeId_WhenUserHasOneWorkItem_ShouldReturnOneItem()
    {
        var assigneeId = Guid.NewGuid();

        GetWorkItemsByAssigneeIdTestsBuilder builder = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(assigneeId)
            .WithWorkItem(assigneeId, "Single work item", "This is the only work item", "High", 5);

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result =
            await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.Count().ShouldBe(1);
        GetWorkItemsByAssigneeIdResponse workItem = result.Result.First();
        workItem.AssigneeId.ShouldBe(assigneeId);
        workItem.Title.ShouldBe("Single work item");
    }

    [Fact, Priority(4)]
    public async Task GetWorkItemsByAssigneeId_WhenUserHasMultipleWorkItems_ShouldReturnAllItems()
    {
        var assigneeId = Guid.NewGuid();

        GetWorkItemsByAssigneeIdTestsBuilder builder = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(assigneeId)
            .WithWorkItem(assigneeId, "First work item", "Description of first item", "High", 5)
            .WithWorkItem(assigneeId, "Second work item", "Description of second item", "Normal", 3)
            .WithWorkItem(assigneeId, "Third work item", "Description of third item", "Low", 8);

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result =
            await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.Count().ShouldBe(3);
        result.Result.All(wi => wi.AssigneeId == assigneeId).ShouldBeTrue();
        result
            .Result.All(wi =>
                wi.Title is "First work item" or "Second work item" or "Third work item"
            )
            .ShouldBeTrue();
        result
            .Result.All(wi =>
                wi.Description
                    is "Description of first item"
                        or "Description of second item"
                        or "Description of third item"
            )
            .ShouldBeTrue();
    }

    [Fact, Priority(5)]
    public async Task GetWorkItemsByAssigneeId_WhenMultipleUsersHaveWorkItems_ShouldReturnOnlyUserItems()
    {
        var assigneeId1 = Guid.NewGuid();
        var assigneeId2 = Guid.NewGuid();

        GetWorkItemsByAssigneeIdTestsBuilder builder = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(assigneeId1)
            .WithWorkItem(assigneeId1, "User 1 - Item 1", "First item for user 1", "High", 5)
            .WithWorkItem(assigneeId1, "User 1 - Item 2", "Second item for user 1", "Normal", 3)
            .WithWorkItem(assigneeId2, "User 2 - Item 1", "First item for user 2", "Low", 4);

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result =
            await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.Count().ShouldBe(2);
        result.Result.All(wi => wi.AssigneeId == assigneeId1).ShouldBeTrue();
        result.Result.Any(wi => wi.AssigneeId == assigneeId2).ShouldBeFalse();
    }

    [Fact, Priority(6)]
    public async Task GetWorkItemsByAssigneeId_WhenCalledMultipleTimes_ShouldReturnConsistentData()
    {
        var assigneeId = Guid.NewGuid();

        GetWorkItemsByAssigneeIdTestsBuilder builder = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(assigneeId)
            .WithWorkItem(
                assigneeId,
                "Consistent work item",
                "This should be consistent",
                "High",
                5
            );

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result1 =
            await builder.ExecuteAsync();

        GetWorkItemsByAssigneeIdTestsBuilder builder2 = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(assigneeId);

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result2 =
            await builder2.ExecuteWithoutWorkItemsAsync();

        result1.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result2.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result1.Result.ShouldNotBeNull();
        result2.Result.ShouldNotBeNull();
        result1.Result.Count().ShouldBe(result2.Result.Count());
        result1.Result.First().Id.ShouldBe(result2.Result.First().Id);
        result1.Result.First().Title.ShouldBe(result2.Result.First().Title);
    }

    [Fact, Priority(7)]
    public async Task GetWorkItemsByAssigneeId_WhenEmptyGuid_ShouldReturnEmptyList()
    {
        Guid emptyGuid = Guid.Empty;

        GetWorkItemsByAssigneeIdTestsBuilder builder = GetWorkItemsByAssigneeIdTestsBuilder
            .New(app)
            .ForAssignee(emptyGuid);

        TestResult<IEnumerable<GetWorkItemsByAssigneeIdResponse>> result =
            await builder.ExecuteWithoutWorkItemsAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.ShouldBeEmpty();
    }
}
