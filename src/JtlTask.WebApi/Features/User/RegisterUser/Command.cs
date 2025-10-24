// <copyright file="Command.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Abstractions;
using Users.Application.Adapters;

namespace JtlTask.WebApi.Features.User.RegisterUser;

/// <summary>
/// Represents a command to register a new user in the system.
/// </summary>
/// <param name="Username">The username for the new user.</param>
internal sealed record RegisterUserCommand(string Username)
    : FastEndpoints.ICommand<Result<UserEntity, IError>>;
