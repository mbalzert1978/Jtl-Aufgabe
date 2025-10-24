// <copyright file="UserDto.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace Users.Application.Adapters;

/// <summary>
/// Data transfer object representing a user in the application layer.
/// Used to transfer user data between application services and presentation layer.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="Username">The username of the user.</param>
public sealed record UserEntity(Guid UserId, string Username);
