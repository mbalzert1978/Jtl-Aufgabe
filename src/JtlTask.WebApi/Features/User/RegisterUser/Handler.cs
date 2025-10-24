// <copyright file="Handler.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using Monads.Results.Extensions.Async;
using SharedKernel.Abstractions;
using Users.Application.Adapters;
using Users.Application.UseCases.RegisterUser;

namespace JtlTask.WebApi.Features.User.RegisterUser;

/// <summary>
/// Handles the execution of register user commands by delegating to the application layer.
/// </summary>
internal sealed class RegisterUserHandler
    : FastEndpoints.ICommandHandler<RegisterUserCommand, Result<UserEntity, IError>>
{
    private readonly SharedKernel.Abstractions.ICommandHandler<
        Users.Application.UseCases.RegisterUser.RegisterUserCommand,
        UserEntity
    > _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserHandler"/> class.
    /// </summary>
    /// <param name="handler">The application-layer command handler.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="handler"/> is null.</exception>
    public RegisterUserHandler(
        SharedKernel.Abstractions.ICommandHandler<
            Users.Application.UseCases.RegisterUser.RegisterUserCommand,
            UserEntity
        > handler
    )
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handler = handler;

        Debug.Assert(_handler == handler, "CommandHandler must be initialized.");
    }

    /// <summary>
    /// Executes the register user command asynchronously.
    /// </summary>
    /// <param name="command">The command containing the user registration data.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, containing the result with either a user entity or an error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public async Task<Result<UserEntity, IError>> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken ct
    )
    {
        ArgumentNullException.ThrowIfNull(command);

        Result<UserEntity, IError> result = await _handler.HandleAsync(new(command.Username), ct);

        Debug.Assert(result is not null, "Result must not be null.");

        return result;
    }
}
