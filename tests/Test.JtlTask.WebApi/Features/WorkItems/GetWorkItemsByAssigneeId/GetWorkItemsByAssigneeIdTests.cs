// <copyright file="GetWorkItemsByAssigneeIdTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.User.RegisterUser;
using JtlTask.WebApi.Features.WorkItems.AssignWorkItem;
using JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId;
using AssignWorkItemEndpoint = JtlTask.WebApi.Features.WorkItems.AssignWorkItem.Endpoint;
using GetWorkItemsByAssigneeIdEndpoint = JtlTask.WebApi.Features.WorkItems.GetWorkItemsByAssigneeId.Endpoint;
using RegisterUserEndpoint = JtlTask.WebApi.Features.User.RegisterUser.Endpoint;

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

        (HttpResponseMessage? rsp, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(nonExistentUserId));

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.ShouldBeEmpty();
    }

    [Fact, Priority(2)]
    public async Task GetWorkItemsByAssigneeId_WhenUserHasNoWorkItems_ShouldReturnEmptyList()
    {
        // Arrange: Use a random assignee ID without creating work items
        var assigneeId = Guid.NewGuid();

        // Act: Get work items for assignee
        (HttpResponseMessage? rsp, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(assigneeId));

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.ShouldBeEmpty();
    }

    [Fact, Priority(3)]
    public async Task GetWorkItemsByAssigneeId_WhenUserHasOneWorkItem_ShouldReturnOneItem()
    {
        // Arrange: Create work item for a specific assignee
        var assigneeId = Guid.NewGuid();

        (_, AssignWorkItemResponse? workItemRes) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId,
                Title: "Single work item",
                Description: "This is the only work item",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        workItemRes.ShouldNotBeNull();

        // Act: Get work items for assignee
        (HttpResponseMessage? rsp, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(assigneeId));

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.Count().ShouldBe(1);
        GetWorkItemsByAssigneeIdResponse workItem = res.First();
        workItem.AssigneeId.ShouldBe(assigneeId);
        workItem.Title.ShouldBe("Single work item");
    }

    [Fact, Priority(4)]
    public async Task GetWorkItemsByAssigneeId_WhenUserHasMultipleWorkItems_ShouldReturnAllItems()
    {
        // Arrange: Assign multiple work items to a specific assignee
        var assigneeId = Guid.NewGuid();

        // Assign first work item
        await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId,
                Title: "First work item",
                Description: "Description of first item",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        // Assign second work item
        await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId,
                Title: "Second work item",
                Description: "Description of second item",
                Priority: "Normal",
                EstimatedHours: 3
            )
        );

        // Assign third work item
        await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId,
                Title: "Third work item",
                Description: "Description of third item",
                Priority: "Low",
                EstimatedHours: 8
            )
        );

        // Act: Get work items for assignee
        (HttpResponseMessage? rsp, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(assigneeId));

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.Count().ShouldBe(3);
        res.All(wi => wi.AssigneeId == assigneeId).ShouldBeTrue();
        res.All(wi => wi.Title is "First work item" or "Second work item" or "Third work item")
            .ShouldBeTrue();
        res.All(wi =>
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
        // Arrange: Assign work items to two different assignees
        var assigneeId1 = Guid.NewGuid();
        var assigneeId2 = Guid.NewGuid();

        // Assign work items to assignee 1
        await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId1,
                Title: "User 1 - Item 1",
                Description: "First item for user 1",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId1,
                Title: "User 1 - Item 2",
                Description: "Second item for user 1",
                Priority: "Normal",
                EstimatedHours: 3
            )
        );

        // Assign work items to assignee 2
        await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId2,
                Title: "User 2 - Item 1",
                Description: "First item for user 2",
                Priority: "Low",
                EstimatedHours: 4
            )
        );

        // Act: Get work items for assignee 1
        (HttpResponseMessage? rsp, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(assigneeId1));

        // Assert
        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.Count().ShouldBe(2);
        // All returned items must belong to assignee 1
        res.All(wi => wi.AssigneeId == assigneeId1).ShouldBeTrue();
        // Should not contain assignee 2's items
        res.Any(wi => wi.AssigneeId == assigneeId2).ShouldBeFalse();
    }

    [Fact, Priority(6)]
    public async Task GetWorkItemsByAssigneeId_WhenCalledMultipleTimes_ShouldReturnConsistentData()
    {
        // Arrange: Assign a work item to a specific assignee
        var assigneeId = Guid.NewGuid();

        (_, AssignWorkItemResponse? workItem) = await app.Client.POSTAsync<
            AssignWorkItemEndpoint,
            AssignWorkItemRequest,
            AssignWorkItemResponse
        >(
            new(
                UserId: assigneeId,
                Title: "Consistent work item",
                Description: "This should be consistent",
                Priority: "High",
                EstimatedHours: 5
            )
        );

        workItem.ShouldNotBeNull();

        // Act: Call the endpoint multiple times
        (HttpResponseMessage? rsp1, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res1) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(assigneeId));

        (HttpResponseMessage? rsp2, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res2) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(assigneeId));

        // Assert: Both calls should return the same data
        rsp1.StatusCode.ShouldBe(HttpStatusCode.OK);
        rsp2.StatusCode.ShouldBe(HttpStatusCode.OK);
        res1.ShouldNotBeNull();
        res2.ShouldNotBeNull();
        res1.Count().ShouldBe(res2.Count());
        res1.First().Id.ShouldBe(res2.First().Id);
        res1.First().Title.ShouldBe(res2.First().Title);
    }

    [Fact, Priority(7)]
    public async Task GetWorkItemsByAssigneeId_WhenEmptyGuid_ShouldReturnEmptyList()
    {
        Guid emptyGuid = Guid.Empty;

        (HttpResponseMessage? rsp, IEnumerable<GetWorkItemsByAssigneeIdResponse>? res) =
            await app.Client.GETAsync<
                GetWorkItemsByAssigneeIdEndpoint,
                GetWorkItemsByAssigneeIdRequest,
                IEnumerable<GetWorkItemsByAssigneeIdResponse>
            >(new(emptyGuid));

        rsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        res.ShouldNotBeNull();
        res.ShouldBeEmpty();
    }
}
