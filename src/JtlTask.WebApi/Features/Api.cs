// <copyright file="Api.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace JtlTask.WebApi.Features;

/// <summary>
/// Defines the base API group configuration for version 1 endpoints.
/// </summary>
internal sealed class Api : Group
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Api"/> class.
    /// </summary>
    public Api()
    {
        Configure(
            "api/v1",
            ep =>
            {
                Debug.Assert(ep is not null, "Endpoint configurator must not be null.");

                ep.Description(b =>
                {
                    Debug.Assert(b is not null, "Description builder must not be null.");

                    b.ProducesProblemFE<InternalErrorResponse>(
                        StatusCodes.Status500InternalServerError
                    );
                });
            }
        );
    }
}
