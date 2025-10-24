// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Net;
using FastEndpoints;
using Monads.Results;
using Monads.Results.Extensions.Async;
using SharedKernel.Abstractions;

namespace JtlTask.WebApi.Features.User.RegisterUser;

/// <summary>
/// Endpoint for registering a new user in the system.
/// </summary>
internal sealed class Endpoint
    : Endpoint<RegisterUserRequest, RegisterUserResponse, RegisterUserMapper>
{
    private const string InternalServerError = "An internal server error occurred.";

    /// <summary>
    /// Configures the endpoint routing, security, and documentation.
    /// </summary>
    public override void Configure()
    {
        Debug.Assert(Config is not null, "Config must be initialized.");

        AllowAnonymous();
        Post("api/v1/users");

        Summary(s =>
        {
            Debug.Assert(s is not null, "Summary configuration object cannot be null.");

            s.Summary = "Register a new user";
            s.Description = "Creates a new user account with the provided username";
            s.Response<RegisterUserResponse>(
                StatusCodes.Status201Created,
                "User successfully registered"
            );
        });

        Options(x =>
        {
            Debug.Assert(x is not null, "Options configuration object cannot be null.");
            x.WithTags("Users");
        });
    }

    /// <summary>
    /// Handles the user registration request.
    /// </summary>
    /// <param name="req">The registration request containing user data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(RegisterUserRequest req, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(req);

        RegisterUserCommand command = new(req.Username);

        Debug.Assert(command is not null, "Command creation must not return null.");
        Debug.Assert(
            command.Username == req.Username,
            "Command username must match request username."
        );

        await command
            .ExecuteAsync(ct)
            .MapAsync(Map.FromEntity)
            .MatchAsync(
                onOk: r => SendSuccessResponseAsync(r, ct),
                onErr: err => HandleErrorAsync(err, ct)
            );
    }

    /// <summary>
    /// Sends a successful response with the registered user data.
    /// </summary>
    /// <param name="response">The response containing the registered user data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task SendSuccessResponseAsync(RegisterUserResponse response, CancellationToken ct)
    {
        Debug.Assert(response is not null, "Response should not be null.");
        Debug.Assert(response.UserId != Guid.Empty, "Response ID must be valid.");
        Debug.Assert(
            !string.IsNullOrWhiteSpace(response.Username),
            "Response username should not be empty."
        );

        await PublishAsync(new UserRegisteredEvent(response.UserId), cancellation: ct);
        await Send.ResponseAsync(response, StatusCodes.Status201Created, ct);
    }

    /// <summary>
    /// Handles errors by mapping them to appropriate HTTP responses.
    /// </summary>
    /// <param name="error">The error to handle.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleErrorAsync(IError error, CancellationToken ct)
    {
        Debug.Assert(error is not null, "Error cannot be null.");
        Debug.Assert(!string.IsNullOrWhiteSpace(error.Message), "Error message cannot be empty.");

        switch (error.Type)
        {
            case ErrorType.NotFound:
                AddError(error.Message);
                await Send.ErrorsAsync(StatusCodes.Status404NotFound, ct);
                break;

            case ErrorType.Validation:
                AddError(error.Message);
                await Send.ErrorsAsync(StatusCodes.Status400BadRequest, ct);
                break;

            default:
                AddError(InternalServerError);
                await Send.ErrorsAsync(StatusCodes.Status500InternalServerError, ct);
                break;
        }
    }
}
