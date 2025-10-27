// <copyright file="RegisterUserTestsBuilder.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using JtlTask.WebApi.Features.User.RegisterUser;

namespace Test.JtlTask.WebApi.Features.Users.RegisterUser;

/// <summary>
/// Builder for constructing RegisterUser test scenarios with a fluent API.
/// </summary>
internal sealed class RegisterUserTestsBuilder
{
    private readonly App _app;
    private Task<TestResult<RegisterUserResponse>>? _registerTask;
    private Task<TestResult<ProblemDetails>>? _problemTask;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserTestsBuilder"/> class.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    private RegisterUserTestsBuilder(App app) => _app = app;

    /// <summary>
    /// Creates a new instance of the <see cref="RegisterUserTestsBuilder"/>.
    /// </summary>
    /// <param name="app">The test application fixture.</param>
    /// <returns>A new builder instance.</returns>
    public static RegisterUserTestsBuilder New(App app)
    {
        Debug.Assert(app is not null, "App fixture must not be null.");
        return new(app);
    }

    /// <summary>
    /// Configures the builder to register a user with a valid username.
    /// </summary>
    /// <param name="username">The username to register.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public RegisterUserTestsBuilder WithValidUser(string username)
    {
        Debug.Assert(
            !string.IsNullOrWhiteSpace(username),
            "Username must not be null or whitespace."
        );
        _registerTask = _app.Client.POSTAsync<Endpoint, RegisterUserRequest, RegisterUserResponse>(
            new(username)
        );
        Debug.Assert(_registerTask is not null, "Registration task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to register a user with an empty username for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public RegisterUserTestsBuilder WithEmptyUsername()
    {
        _problemTask = _app.Client.POSTAsync<Endpoint, RegisterUserRequest, ProblemDetails>(
            new(string.Empty)
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to register a user with a username below the minimum length for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public RegisterUserTestsBuilder WithUsernameTooShort()
    {
        _problemTask = _app.Client.POSTAsync<Endpoint, RegisterUserRequest, ProblemDetails>(
            new("ab")
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to register a user with a username exceeding the maximum length for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public RegisterUserTestsBuilder WithUsernameTooLong()
    {
        _problemTask = _app.Client.POSTAsync<Endpoint, RegisterUserRequest, ProblemDetails>(
            new(new string('a', 201))
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to register a user with invalid special characters in the username for testing validation.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public RegisterUserTestsBuilder WithInvalidCharacters()
    {
        _problemTask = _app.Client.POSTAsync<Endpoint, RegisterUserRequest, ProblemDetails>(
            new("user@name!")
        );
        Debug.Assert(_problemTask is not null, "Problem task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to register a user with a username at the minimum valid length.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public RegisterUserTestsBuilder WithMinimumLengthUsername()
    {
        _registerTask = _app.Client.POSTAsync<Endpoint, RegisterUserRequest, RegisterUserResponse>(
            new(new string('a', 3))
        );
        Debug.Assert(_registerTask is not null, "Registration task must not be null.");
        return this;
    }

    /// <summary>
    /// Configures the builder to register a user with a username at the maximum valid length.
    /// </summary>
    /// <returns>The builder instance for method chaining.</returns>
    public RegisterUserTestsBuilder WithMaximumLengthUsername()
    {
        _registerTask = _app.Client.POSTAsync<Endpoint, RegisterUserRequest, RegisterUserResponse>(
            new(new string('a', 200))
        );
        Debug.Assert(_registerTask is not null, "Registration task must not be null.");
        return this;
    }

    /// <summary>
    /// Executes the registration request expecting a successful response.
    /// </summary>
    /// <returns>The registration response.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the registration task has not been initialized.</exception>
    public async Task<TestResult<RegisterUserResponse>> ExecuteAsync()
    {
        if (_registerTask is null)
            throw new InvalidOperationException(
                "Registration task has not been initialized. Call a With* method before executing."
            );

        TestResult<RegisterUserResponse> result = await _registerTask.ConfigureAwait(false);
        Debug.Assert(result is not null, "Result must not be null after execution.");
        return result;
    }

    /// <summary>
    /// Executes the registration request expecting a validation error response.
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
