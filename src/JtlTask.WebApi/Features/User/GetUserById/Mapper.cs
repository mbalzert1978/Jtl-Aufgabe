// <copyright file="Mapper.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using FastEndpoints;
using JtlTask.WebApi.Features.User.RegisterUser;
using Users.Application.Adapters;

namespace JtlTask.WebApi.Features.User.GetUserById;

/// <summary>
/// Maps between <see cref="RegisterUserRequest"/>, <see cref="RegisterUserResponse"/>, and <see cref="UserEntity"/>.
/// </summary>
internal sealed class GetUserByIdMapper
    : Mapper<GetUserByIdRequest, GetUserByIdResponse, UserEntity>
{
    public override GetUserByIdResponse FromEntity(UserEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        GetUserByIdResponse response = new(
            entity.UserId,
            entity.Username,
            entity.Tasks.Select(t => t.TaskId)
        );

        Debug.Assert(response is not null, "Response creation must not return null.");
        Debug.Assert(
            response.UserId == entity.UserId,
            "Mapped response ID must match entity UserId."
        );
        Debug.Assert(
            response.Username == entity.Username,
            "Mapped response Username must match entity Username."
        );

        return response;
    }
}
