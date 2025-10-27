// <copyright file="Models.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using FastEndpoints;
using FluentValidation;

namespace JtlTask.WebApi.Features.User.RegisterUser;

/// <summary>
/// Represents a request to register a new user.
/// </summary>
/// <param name="Username">The username for the new user account.</param>
internal sealed record RegisterUserRequest(string Username);

/// <summary>
/// Represents the response for a successful user registration.
/// </summary>
/// <param name="UserId">The unique identifier of the newly registered user.</param>
/// <param name="Username">The username of the newly registered user.</param>
internal sealed record RegisterUserResponse(Guid UserId, string Username);

/// <summary>
/// Event published when a user is successfully registered.
/// </summary>
/// <param name="UserId">The unique identifier of the registered user.</param>
internal sealed record UserRegisteredEvent(Guid UserId) : IEvent;

/// <summary>
/// Validator for <see cref="RegisterUserRequest"/>.
/// </summary>
internal sealed class RegisterUserRequestValidator : Validator<RegisterUserRequest>
{
    private const int MaxUsernameLength = 200;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserRequestValidator"/> class.
    /// </summary>
    public RegisterUserRequestValidator() =>
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .MaximumLength(MaxUsernameLength)
            .WithMessage($"Username must be less than {MaxUsernameLength} characters long.");
}
