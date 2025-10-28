// <copyright file="Handler.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Users.Application.Adapters;
using Users.Infrastructure.Persistence;

namespace JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// Handles the execution of register user commands by delegating to the application layer.
/// </summary>
internal sealed class GetUserByIdHandler : ICommandHandler<GetUserByIdQuery, UserEntity?>
{
    private readonly UsersDbContext _context;

    public GetUserByIdHandler(UsersDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;

        Debug.Assert(_context == context, "DbContext must be initialized.");
    }

    public async Task<UserEntity?> ExecuteAsync(GetUserByIdQuery command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);

        UserEntity? user = await _context
            .Users.FindAsync([command.UserId], ct)
            .ConfigureAwait(false);
        Debug.Assert(
            user is null || user.UserId == command.UserId,
            "Retrieved user's ID must match the requested UserId."
        );

        return user;
    }
}
