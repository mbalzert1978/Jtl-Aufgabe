// <copyright file="RegisterUserTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.Users.RegisterUser;

namespace Test.JtlTask.WebApi.Features.Users.RegisterUser;

/// <summary>
/// End-to-end tests for the RegisterUser endpoint.
/// </summary>
public class RegisterUserTests(App app) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task RegisterUser_WhenUsernameIsEmpty_ShouldReturnBadRequest()
    {
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder.New(app).WithEmptyUsername();

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(2)]
    public async Task RegisterUser_WhenUsernameTooShort_ShouldReturnBadRequest()
    {
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder.New(app).WithUsernameTooShort();

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(3)]
    public async Task RegisterUser_WhenUsernameTooLong_ShouldReturnBadRequest()
    {
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder.New(app).WithUsernameTooLong();

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(4)]
    public async Task RegisterUser_WhenUsernameContainsInvalidCharacters_ShouldReturnBadRequest()
    {
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder
            .New(app)
            .WithInvalidCharacters();

        TestResult<ProblemDetails> result = await builder.ExecuteProblemAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Result.Errors.ShouldNotBeEmpty();
        result.Result.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(5)]
    public async Task RegisterUser_WhenValidUsername_ShouldReturnCreatedWithUserData()
    {
        string username = "validUser123";
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder
            .New(app)
            .WithValidUser(username);

        TestResult<RegisterUserResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.UserId.ShouldNotBe(Guid.Empty);
        result.Result.Username.ShouldBe(username);
    }

    [Fact, Priority(6)]
    public async Task RegisterUser_WhenValidUsernameWithUnderscores_ShouldReturnCreated()
    {
        string username = "valid_user_123";
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder
            .New(app)
            .WithValidUser(username);

        TestResult<RegisterUserResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.UserId.ShouldNotBe(Guid.Empty);
        result.Result.Username.ShouldBe(username);
    }

    [Fact, Priority(7)]
    public async Task RegisterUser_WhenValidUsernameWithHyphens_ShouldReturnCreated()
    {
        string username = "valid-user-456";
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder
            .New(app)
            .WithValidUser(username);

        TestResult<RegisterUserResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.UserId.ShouldNotBe(Guid.Empty);
        result.Result.Username.ShouldBe(username);
    }

    [Fact, Priority(8)]
    public async Task RegisterUser_WhenMinimumLengthUsername_ShouldReturnCreated()
    {
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder
            .New(app)
            .WithMinimumLengthUsername();

        TestResult<RegisterUserResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.UserId.ShouldNotBe(Guid.Empty);
        result.Result.Username.Length.ShouldBe(3);
    }

    [Fact, Priority(9)]
    public async Task RegisterUser_WhenMaximumLengthUsername_ShouldReturnCreated()
    {
        RegisterUserTestsBuilder builder = RegisterUserTestsBuilder
            .New(app)
            .WithMaximumLengthUsername();

        TestResult<RegisterUserResponse> result = await builder.ExecuteAsync();

        result.Response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Result.ShouldNotBeNull();
        result.Result.UserId.ShouldNotBe(Guid.Empty);
        result.Result.Username.Length.ShouldBe(200);
    }
}
