// <copyright file="Mapper.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using JtlTask.WebApi.Features.Users.RegisterUser;
using Users.Application.Adapters;

namespace JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// Maps between <see cref="RegisterUserRequest"/>, <see cref="RegisterUserResponse"/>, and <see cref="UserEntity"/>.
/// </summary>
internal sealed class GetUserByIdMapper
    : Mapper<GetUserByIdRequest, GetUserByIdResponse, UserEntity>
{
    public override GetUserByIdResponse FromEntity(UserEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        GetUserByIdResponse response = new(entity.UserId, entity.Username);

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
