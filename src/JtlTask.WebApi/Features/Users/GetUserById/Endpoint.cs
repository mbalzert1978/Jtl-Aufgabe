// <copyright file="Endpoint.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
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
        Get("/{UserId:guid}");
        Group<Route>();

        Description(b =>
        {
            Debug.Assert(b is not null, "Description configuration object cannot be null.");
            b.Produces<GetUserByIdResponse>(StatusCodes.Status200OK);
            b.Produces<ProblemDetails>(StatusCodes.Status404NotFound);
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
                AddError($"User with ID '{req.UserId}' was not found.", "UserNotFound");
                await Send.ErrorsAsync(StatusCodes.Status404NotFound, ct);
                return;
        }
    }
}
