// <copyright file="RegisterUserTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using JtlTask.WebApi.Features.User.RegisterUser;

namespace Test.JtlTask.WebApi.Features.Users.RegisterUser;

/// <summary>
/// End-to-end tests for the RegisterUser endpoint.
/// </summary>
public class RegisterUserTests(App app) : TestBase<App>
{
    [Fact, Priority(1)]
    public async Task RegisterUser_WhenUsernameIsEmpty_ShouldReturnBadRequest()
    {
        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            ProblemDetails
        >(new(string.Empty));

        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(2)]
    public async Task RegisterUser_WhenUsernameTooShort_ShouldReturnBadRequest()
    {
        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            ProblemDetails
        >(new("ab"));

        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(3)]
    public async Task RegisterUser_WhenUsernameTooLong_ShouldReturnBadRequest()
    {
        string longUsername = new('a', 201);

        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            ProblemDetails
        >(new(longUsername));

        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(4)]
    public async Task RegisterUser_WhenUsernameContainsInvalidCharacters_ShouldReturnBadRequest()
    {
        (HttpResponseMessage? rsp, ProblemDetails? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            ProblemDetails
        >(new("user@name!"));

        rsp.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        res.Errors.ShouldNotBeEmpty();
        res.Errors.Any(e => e.Name == "username").ShouldBeTrue();
    }

    [Fact, Priority(5)]
    public async Task RegisterUser_WhenValidUsername_ShouldReturnCreatedWithUserData()
    {
        string username = "validUser123";

        (HttpResponseMessage? rsp, RegisterUserResponse? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(new(username));

        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.UserId.ShouldNotBe(Guid.Empty);
        res.Username.ShouldBe(username);
    }

    [Fact, Priority(6)]
    public async Task RegisterUser_WhenValidUsernameWithUnderscores_ShouldReturnCreated()
    {
        string username = "valid_user_123";

        (HttpResponseMessage? rsp, RegisterUserResponse? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(new(username));

        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.UserId.ShouldNotBe(Guid.Empty);
        res.Username.ShouldBe(username);
    }

    [Fact, Priority(7)]
    public async Task RegisterUser_WhenValidUsernameWithHyphens_ShouldReturnCreated()
    {
        string username = "valid-user-456";

        (HttpResponseMessage? rsp, RegisterUserResponse? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(new(username));

        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.UserId.ShouldNotBe(Guid.Empty);
        res.Username.ShouldBe(username);
    }

    [Fact, Priority(8)]
    public async Task RegisterUser_WhenMinimumLengthUsername_ShouldReturnCreated()
    {
        string username = "abc";

        (HttpResponseMessage? rsp, RegisterUserResponse? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(new(username));

        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.UserId.ShouldNotBe(Guid.Empty);
        res.Username.ShouldBe(username);
    }

    [Fact, Priority(9)]
    public async Task RegisterUser_WhenMaximumLengthUsername_ShouldReturnCreated()
    {
        string username = new('a', 200);

        (HttpResponseMessage? rsp, RegisterUserResponse? res) = await app.Client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(new(username));

        rsp.StatusCode.ShouldBe(HttpStatusCode.Created);
        res.ShouldNotBeNull();
        res.UserId.ShouldNotBe(Guid.Empty);
        res.Username.ShouldBe(username);
    }
}
