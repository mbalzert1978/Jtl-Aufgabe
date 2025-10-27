// <copyright file="GetUserByIdTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.User.RegisterUser;
using JtlTask.WebApi.Features.Users.GetUserById;
using GetUserByIdEndpoint = JtlTask.WebApi.Features.Users.GetUserById.Endpoint;
using RegisterUserEndpoint = JtlTask.WebApi.Features.User.RegisterUser.Endpoint;

namespace Test.JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// End-to-end tests for the GetUserById endpoint.
/// </summary>
public class GetUserByIdTests(App app) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        var nonExistentUserId = Guid.NewGuid();

        HttpResponseMessage rsp = await app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest
        >(new(nonExistentUserId));

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact, Priority(2)]
    public async Task GetUserById_WhenUserExists_ShouldReturnOkWithUserData()
    {
        // Arrange: First create a user
        string username = "testUser_getById";
        (HttpResponseMessage? createRsp, RegisterUserResponse? createRes) =
            await app.Client.POSTAsync<
                RegisterUserEndpoint,
                RegisterUserRequest,
                RegisterUserResponse
            >(new(username));

        createRsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        createRes.ShouldNotBeNull();
        Guid createdUserId = createRes.UserId;

        // Act: Retrieve the user by ID
        (HttpResponseMessage? getRsp, GetUserByIdResponse? getRes) = await app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest,
            GetUserByIdResponse
        >(new(createdUserId));

        // Assert
        getRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        getRes.ShouldNotBeNull();
        getRes.UserId.ShouldBe(createdUserId);
        getRes.Username.ShouldBe(username);
    }

    [Fact, Priority(3)]
    public async Task GetUserById_WhenEmptyGuid_ShouldReturnNotFound()
    {
        Guid emptyGuid = Guid.Empty;

        HttpResponseMessage rsp = await app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest
        >(new(emptyGuid));

        rsp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact, Priority(4)]
    public async Task GetUserById_WhenMultipleUsersExist_ShouldReturnCorrectUser()
    {
        // Arrange: Create multiple users
        string username1 = "user1_getById";
        string username2 = "user2_getById";

        (HttpResponseMessage? createRsp1, RegisterUserResponse? createRes1) =
            await app.Client.POSTAsync<
                RegisterUserEndpoint,
                RegisterUserRequest,
                RegisterUserResponse
            >(new(username1));

        (HttpResponseMessage? createRsp2, RegisterUserResponse? createRes2) =
            await app.Client.POSTAsync<
                RegisterUserEndpoint,
                RegisterUserRequest,
                RegisterUserResponse
            >(new(username2));

        createRsp1.StatusCode.ShouldBe(HttpStatusCode.Created);
        createRsp2.StatusCode.ShouldBe(HttpStatusCode.Created);
        createRes1.ShouldNotBeNull();
        createRes2.ShouldNotBeNull();

        Guid userId1 = createRes1.UserId;
        Guid userId2 = createRes2.UserId;

        // Act: Retrieve the second user
        (HttpResponseMessage? getRsp, GetUserByIdResponse? getRes) = await app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest,
            GetUserByIdResponse
        >(new(userId2));

        // Assert
        getRsp.StatusCode.ShouldBe(HttpStatusCode.OK);
        getRes.ShouldNotBeNull();
        getRes.UserId.ShouldBe(userId2);
        getRes.Username.ShouldBe(username2);
        getRes.UserId.ShouldNotBe(userId1);
    }

    [Fact, Priority(5)]
    public async Task GetUserById_WhenCalledMultipleTimes_ShouldReturnSameData()
    {
        // Arrange: Create a user
        string username = "consistent_user";
        (HttpResponseMessage? createRsp, RegisterUserResponse? createRes) =
            await app.Client.POSTAsync<
                RegisterUserEndpoint,
                RegisterUserRequest,
                RegisterUserResponse
            >(new(username));

        createRsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        createRes.ShouldNotBeNull();
        Guid userId = createRes.UserId;

        // Act: Call the endpoint multiple times
        (HttpResponseMessage? getRsp1, GetUserByIdResponse? getRes1) = await app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest,
            GetUserByIdResponse
        >(new(userId));

        (HttpResponseMessage? getRsp2, GetUserByIdResponse? getRes2) = await app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest,
            GetUserByIdResponse
        >(new(userId));

        // Assert: Both calls should return the same data
        getRsp1.StatusCode.ShouldBe(HttpStatusCode.OK);
        getRsp2.StatusCode.ShouldBe(HttpStatusCode.OK);
        getRes1.ShouldNotBeNull();
        getRes2.ShouldNotBeNull();
        getRes1.UserId.ShouldBe(getRes2.UserId);
        getRes1.Username.ShouldBe(getRes2.Username);
    }
}
