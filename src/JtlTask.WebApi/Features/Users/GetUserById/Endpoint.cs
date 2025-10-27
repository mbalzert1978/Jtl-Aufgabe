// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Net;
using FastEndpoints;
using Users.Application.Adapters;

namespace JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// Endpoint for registering a new user in the system.
/// </summary>
internal sealed class Endpoint
    : Endpoint<GetUserByIdRequest, GetUserByIdResponse, GetUserByIdMapper>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    public override void Configure()
    {
        AllowAnonymous();
        Get("api/v1/users/{UserId:guid}");

        Summary(s =>
        {
            Debug.Assert(s is not null, "Summary configuration object cannot be null.");

            s.Summary = "Get user by ID";
            s.Description = "Retrieves a user by their unique identifier";
            s.Response<GetUserByIdResponse>(StatusCodes.Status200OK, "User successfully retrieved");
            s.Response(StatusCodes.Status404NotFound, "User with the specified ID does not exist");
        });

        Options(x =>
        {
            Debug.Assert(x is not null, "Options configuration object cannot be null.");
            x.WithTags("Users");
        });
    }

    public override async Task HandleAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(req);

        GetUserByIdQuery query = new(req.UserId);

        switch (await query.ExecuteAsync(ct))
        {
            case UserEntity user:
                await Send.OkAsync(Map.FromEntity(user), ct);
                break;
            default:
                await Send.NotFoundAsync(ct);
                return;
        }
    }
}
