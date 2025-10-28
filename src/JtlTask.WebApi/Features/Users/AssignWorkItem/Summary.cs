// <copyright file="Summary.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace JtlTask.WebApi.Features.Users.AssignWorkItem;

/// <summary>
/// Provides API documentation summary for the AssignWorkItem endpoint.
/// </summary>
internal sealed class AssignWorkItemSummary : Summary<Endpoint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AssignWorkItemSummary"/> class.
    /// </summary>
    public AssignWorkItemSummary()
    {
        Summary = "Assign a work item to a assignee";
        Description = "Assigns a new work item to the specified assignee";

        Response<AssignWorkItemResponse>(
            StatusCodes.Status201Created,
            "The work item was successfully assigned."
        );

        Response<ProblemDetails>(StatusCodes.Status400BadRequest, "The request was invalid.");

        Response<ProblemDetails>(
            StatusCodes.Status404NotFound,
            "The specified assignee was not found."
        );

        Response<InternalErrorResponse>(
            StatusCodes.Status500InternalServerError,
            "An internal server error occurred."
        );

        Debug.Assert(
            RequestExamples is not null,
            "RequestExamples must not be null after base initialization."
        );
        RequestExamples.Add(
            new(
                new AssignWorkItemRequest(
                    Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    "Implement authentication",
                    "This task involves setting up user authentication using OAuth2.",
                    "High",
                    16,
                    DateTimeOffset.UtcNow.AddDays(14),
                    Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ["backend", "security"]
                )
            )
        );

        Debug.Assert(
            ResponseExamples is not null,
            "ResponseExamples must not be null after base initialization."
        );
        ResponseExamples[StatusCodes.Status201Created] = new AssignWorkItemResponse(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Implement authentication",
            "This task involves setting up user authentication using OAuth2.",
            "InProgress",
            "High",
            new DateTimeOffset(2025, 10, 31, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 10, 28, 12, 0, 0, TimeSpan.Zero),
            14,
            ["backend", "security"],
            Guid.Parse("22222222-2222-2222-2222-222222222222")
        );
    }
}
