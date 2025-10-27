// <copyright file="GetUserByIdTestsBuilder.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using JtlTask.WebApi.Features.User.RegisterUser;
using JtlTask.WebApi.Features.Users.GetUserById;
using GetUserByIdEndpoint = JtlTask.WebApi.Features.Users.GetUserById.Endpoint;
using RegisterUserEndpoint = JtlTask.WebApi.Features.User.RegisterUser.Endpoint;

namespace Test.JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// Builder for constructing GetUserById test scenarios with a fluent API.
/// </summary>
internal sealed class GetUserByIdTestsBuilder
{
    private readonly App _app;
    private readonly List<Task<TestResult<RegisterUserResponse>>> _tasks = [];
    private Task<TestResult<GetUserByIdResponse>>? _getTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdTestsBuilder"/> class.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    private GetUserByIdTestsBuilder(App app) => _app = app;

    /// <summary>
    /// Creates a new instance of the <see cref="GetUserByIdTestsBuilder"/>.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    /// <returns>A new builder instance.</returns>
    public static GetUserByIdTestsBuilder New(App app)
    {
        Debug.Assert(app is not null, "App fixture must not be null.");
        return new(app);
    }

    /// <summary>
    /// Registers a user with the specified username.
    /// </summary>
    /// <param name="input">The username to register. Defaults to a test user.</param>
    private void RegisterUserById(string input = "testUser_getByIdBuilder")
    {
        Debug.Assert(input is not null, "Username input must not be null.");
        _tasks.Add(
            _app.Client.POSTAsync<RegisterUserEndpoint, RegisterUserRequest, RegisterUserResponse>(
                new(input)
            )
        );
        Debug.Assert(_tasks is not null, "Registration task must not be null.");
    }

    /// <summary>
    /// Configures the builder to create a valid user for testing.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public GetUserByIdTestsBuilder WithValidUser(string username = "testUser_getByIdBuilder")
    {
        RegisterUserById(username);
        Debug.Assert(_tasks is not null, "Task must be initialized after registration.");
        return this;
    }

    /// <summary>
    /// Configures the builder to attempt to get a user by a non-existent ID for testing not found scenario.
    /// </summary>
    public GetUserByIdTestsBuilder WithInvalidUserNotFound()
    {
        _getTask = _app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest,
            GetUserByIdResponse
        >(new(Guid.NewGuid()));
        Debug.Assert(_getTask is not null, "GET request task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to create a user with an empty username for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public GetUserByIdTestsBuilder WithInvalidUserNameEmpty()
    {
        RegisterUserById(string.Empty);
        Debug.Assert(_tasks is not null, "Task must be initialized after registration.");
        return this;
    }

    /// <summary>
    /// Configures the builder to create a user with a username below the minimum length for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public GetUserByIdTestsBuilder WithInvalidUserNameMinLength()
    {
        RegisterUserById("ab");
        Debug.Assert(_tasks is not null, "Task must be initialized after registration.");
        return this;
    }

    /// <summary>
    /// Configures the builder to create a user with a username exceeding the maximum length for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public GetUserByIdTestsBuilder WithInvalidUserNameMaxLength()
    {
        RegisterUserById(new string('a', 201));
        Debug.Assert(_tasks is not null, "Task must be initialized after registration.");
        return this;
    }

    /// <summary>
    /// Configures the builder to create a user with invalid special characters in the username for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public GetUserByIdTestsBuilder WithInvalidUserNameSpecialChars()
    {
        RegisterUserById("invalid!user@name#");
        Debug.Assert(_tasks is not null, "Task must be initialized after registration.");
        return this;
    }

    /// <summary>
    /// Builds and executes the test scenario, returning the created user response.
    /// </summary>
    /// <returns>The created user response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no user configuration has been specified or user creation failed.</exception>
    public async Task<RegisterUserResponse> BuildAsync()
    {
        RegisterUserResponse userResponse = (_tasks, _getTask) switch
        {
            ({ Count: 0 }, not null) => throw new InvalidOperationException(
                "Cannot build user response when testing non-existent user scenarios. Use ExecuteAsync() directly."
            ),
            ({ Count: 0 }, null) => throw new InvalidOperationException(
                "No user configuration specified. Call WithValidUser() or another With* method before building."
            ),
            ({ Count: > 0 }, _) => await CreateAndPrepareGetRequestAsync(),
        };

        Debug.Assert(userResponse is not null, "User response must not be null after build.");
        Debug.Assert(userResponse.UserId != Guid.Empty, "Created user ID must not be empty.");

        return userResponse;
    }

    /// <summary>
    /// Creates the user and prepares the GET request for retrieval.
    /// </summary>
    /// <returns>The created user response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when user creation fails.</exception>
    private async Task<RegisterUserResponse> CreateAndPrepareGetRequestAsync()
    {
        Debug.Assert(_tasks is not null, "Registration task must not be null.");

        TestResult<RegisterUserResponse>[] results = await Task.WhenAll(_tasks)
            .ConfigureAwait(false);
        TestResult<RegisterUserResponse> result = results[^1];

        if (!result.Response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"User creation failed during test setup with status code: {result.Response.StatusCode}"
            );

        Debug.Assert(
            result.Result is not null,
            "Result must not be null after successful creation."
        );

        _getTask = _app.Client.GETAsync<
            GetUserByIdEndpoint,
            GetUserByIdRequest,
            GetUserByIdResponse
        >(new(result.Result.UserId));

        Debug.Assert(_getTask is not null, "GET request task must not be null.");

        return result.Result;
    }

    /// <summary>
    /// Executes the GetUserById request and returns the response.
    /// </summary>
    /// <returns>The GetUserById response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the GetUserById task has not been initialized.</exception>
    public async Task<TestResult<GetUserByIdResponse>> ExecuteAsync()
    {
        if (_getTask is null)
            throw new InvalidOperationException(
                "GetUserById task has not been initialized. Ensure BuildAsync() has been called."
            );

        return await _getTask.ConfigureAwait(false);
    }
}
