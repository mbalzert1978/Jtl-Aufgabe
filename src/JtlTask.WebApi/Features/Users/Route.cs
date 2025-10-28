// <copyright file="Route.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace JtlTask.WebApi.Features.Users;

/// <summary>
/// Defines the routing configuration for user-related endpoints.
/// </summary>
internal sealed class Route : SubGroup<Api>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Route"/> class.
    /// </summary>
    public Route()
    {
        Configure(
            "users",
            ep =>
            {
                Debug.Assert(ep is not null, "Endpoint configurator must not be null.");

                ep.Description(b =>
                {
                    Debug.Assert(b is not null, "Description builder must not be null.");

                    b.WithTags("Users");
                });
            }
        );
    }
}
