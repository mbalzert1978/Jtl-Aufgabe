// <copyright file="Query.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Users.Application.Adapters;

namespace JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// Represents a query to retrieve a user by their unique identifier.
/// </summary>
/// <param name="UserId">The unique identifier of the user to retrieve.</param>
internal sealed record GetUserByIdQuery(Guid UserId) : ICommand<UserEntity?>;
