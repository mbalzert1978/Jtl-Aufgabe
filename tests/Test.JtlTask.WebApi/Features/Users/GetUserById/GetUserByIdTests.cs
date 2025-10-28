// <copyright file="GetUserByIdTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.Users.GetUserById;
using JtlTask.WebApi.Features.Users.RegisterUser;

namespace Test.JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// End-to-end tests for the GetUserById endpoint.
/// </summary>
public class GetUserByIdTests(App app) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task GetUserById_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        GetUserByIdTestsBuilder builder = GetUserByIdTestsBuilder
            .New(app)
            .WithInvalidUserNotFound();

        TestResult<GetUserByIdResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact, Priority(2)]
    public async Task GetUserById_WhenUserExists_ShouldReturnOkWithUserData()
    {
        GetUserByIdTestsBuilder builder = GetUserByIdTestsBuilder.New(app).WithValidUser();
        RegisterUserResponse userResponse = await builder.BuildAsync();

        TestResult<GetUserByIdResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.UserId.ShouldBe(userResponse.UserId);
        result.Result.Username.ShouldBe(userResponse.Username);
    }

    [Fact, Priority(3)]
    public async Task GetUserById_WhenMultipleUsersExist_ShouldReturnCorrectUser()
    {
        GetUserByIdTestsBuilder builder = GetUserByIdTestsBuilder
            .New(app)
            .WithValidUser("testUser1_getByIdBuilder")
            .WithValidUser("testUser2_getByIdBuilder");

        RegisterUserResponse lastUser = await builder.BuildAsync();

        TestResult<GetUserByIdResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Result.ShouldNotBeNull();
        result.Result.UserId.ShouldBe(lastUser.UserId);
        result.Result.Username.ShouldNotBe("testUser1_getByIdBuilder");
        result.Result.Username.ShouldBe("testUser2_getByIdBuilder");
    }

    [Fact, Priority(4)]
    public async Task GetUserById_WhenCalledMultipleTimes_ShouldReturnSameData()
    {
        GetUserByIdTestsBuilder builder = GetUserByIdTestsBuilder.New(app).WithValidUser();

        RegisterUserResponse user = await builder.BuildAsync();

        TestResult<GetUserByIdResponse> result1 = await builder.ExecuteAsync();
        TestResult<GetUserByIdResponse> result2 = await builder.ExecuteAsync();

        result1.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result2.Response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result1.Result.ShouldNotBeNull();
        result2.Result.ShouldNotBeNull();
        result1.Result.UserId.ShouldBe(result2.Result.UserId);
        result1.Result.Username.ShouldBe(result2.Result.Username);
        result1.Result.UserId.ShouldBe(user.UserId);
    }
}
