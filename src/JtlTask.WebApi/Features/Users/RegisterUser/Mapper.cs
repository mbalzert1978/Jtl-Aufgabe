// <copyright file="Mapper.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Users.Application.Adapters;

namespace JtlTask.WebApi.Features.Users.RegisterUser;

/// <summary>
/// Maps between <see cref="RegisterUserRequest"/>, <see cref="RegisterUserResponse"/>, and <see cref="UserEntity"/>.
/// </summary>
internal sealed class RegisterUserMapper
    : Mapper<RegisterUserRequest, RegisterUserResponse, UserEntity>
{
    /// <summary>
    /// Converts a <see cref="UserEntity"/> to a <see cref="RegisterUserResponse"/>.
    /// </summary>
    /// <param name="entity">The entity to convert.</param>
    /// <returns>A <see cref="RegisterUserResponse"/> representing the entity data.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is null.</exception>
    public override RegisterUserResponse FromEntity(UserEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        RegisterUserResponse response = new(entity.UserId, entity.Username);

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
