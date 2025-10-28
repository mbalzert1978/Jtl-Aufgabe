// <copyright file="Summary.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace JtlTask.WebApi.Features.Users.GetWorkItemsByAssigneeId;

/// <summary>
/// Defines the OpenAPI summary for the get work items by assignee ID endpoint.
/// </summary>
internal sealed class GetWorkItemsSummary : Summary<Endpoint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetWorkItemsSummary"/> class.
    /// </summary>
    public GetWorkItemsSummary()
    {
        Summary = "Get work items by assignee ID";
        Description = "Retrieves all work items assigned to a specific user";

        Response<IEnumerable<GetWorkItemsByAssigneeIdResponse>>(
            StatusCodes.Status200OK,
            "Work items successfully retrieved"
        );

        Response(
            StatusCodes.Status200OK,
            "Returns empty list if no work items found for the assignee"
        );

        Response<InternalErrorResponse>(
            StatusCodes.Status500InternalServerError,
            "An internal server error occurred."
        );

        Debug.Assert(RequestExamples is not null, "RequestExamples must not be null.");
        RequestExamples.Add(
            new(
                new GetWorkItemsByAssigneeIdRequest(
                    Guid.Parse("11111111-1111-1111-1111-111111111111")
                )
            )
        );

        Debug.Assert(ResponseExamples is not null, "ResponseExamples must not be null.");
        ResponseExamples[StatusCodes.Status200OK] = new[]
        {
            new GetWorkItemsByAssigneeIdResponse(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "Implement authentication",
                "This task involves setting up user authentication using OAuth2.",
                "InProgress",
                "High",
                new DateTimeOffset(2025, 10, 31, 12, 0, 0, TimeSpan.Zero),
                null,
                14,
                ["backend", "security"],
                Guid.Parse("33333333-3333-3333-3333-333333333333")
            ),
            new GetWorkItemsByAssigneeIdResponse(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Implement Feature X",
                "This task involves developing Feature X according to the specifications.",
                "Completed",
                "High",
                new DateTimeOffset(2025, 10, 28, 12, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2025, 10, 31, 12, 0, 0, TimeSpan.Zero),
                14,
                ["backend", "feature-x", "urgent"],
                Guid.Empty
            ),
        };
    }
}
