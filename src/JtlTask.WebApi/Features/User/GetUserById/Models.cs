// <copyright file="Models.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace JtlTask.WebApi.Features.User.GetUserById;

/// <summary>
/// Request to get a user by their unique identifier.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
internal sealed record GetUserByIdRequest(Guid UserId);

/// <summary>
/// Response containing user information.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="Username">The username of the user.</param>
internal sealed record GetUserByIdResponse(Guid UserId, string Username);
