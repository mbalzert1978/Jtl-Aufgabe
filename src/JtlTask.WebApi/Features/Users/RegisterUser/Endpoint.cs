// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Mediator;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using SharedKernel.Abstractions;
using Users.Application.Adapters;
using Users.Application.UseCases.RegisterUser;

namespace JtlTask.WebApi.Features.Users.RegisterUser;

/// <summary>
/// Endpoint for registering a new user in the system.
/// </summary>
internal sealed class Endpoint
    : Endpoint<RegisterUserRequest, RegisterUserResponse, RegisterUserMapper>
{
    private const string InternalServerError = "An internal server error occurred.";
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);

        _mediator = mediator;

        Debug.Assert(_mediator == mediator, "Mediator assignment must be successful.");
    }

    /// <summary>
    /// Configures the endpoint routing, security, and documentation.
    /// </summary>
    public override void Configure()
    {
        Debug.Assert(Config is not null, "Config must be initialized.");

        AllowAnonymous();
        Post("/");
        Group<Route>();

        Description(b =>
        {
            Debug.Assert(b is not null, "Description configuration object cannot be null.");
            b.Produces<RegisterUserResponse>(StatusCodes.Status201Created);
            b.ProducesProblemDetails(StatusCodes.Status400BadRequest);
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

        RegisterUserCommand cmd = new(req.Username);
        Debug.Assert(cmd.Username == req.Username, "Command username must match request.");
        // csharpier-ignore
        Result<UserEntity, IError> result =
            await _mediator.SendAsync<RegisterUserCommand, UserEntity>(cmd, ct);

        await result
            .Map(Map.FromEntity)
            .Match(
                response => Send.ResponseAsync(response, StatusCodes.Status201Created, ct),
                error =>
                {
                    switch (error.ErrorType)
                    {
                        case ErrorType.Validation:
                            AddError(error.Message);
                            Send.ErrorsAsync(StatusCodes.Status400BadRequest, ct);
                            break;

                        default:
                            AddError(InternalServerError);
                            Send.ErrorsAsync(StatusCodes.Status500InternalServerError, ct);
                            break;
                    }

                    return Task.CompletedTask;
                }
            );
    }
}
