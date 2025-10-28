// <copyright file="RegisterUserCommand.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Mediator;
using Users.Application.Adapters;

namespace Users.Application.UseCases.RegisterUser;

/// <summary>
/// Command to register a new user in the system.
/// </summary>
/// <param name="Username">The username for the new user.</param>
public sealed record RegisterUserCommand(string Username) : ICommand<UserEntity>;
