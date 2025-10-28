// <copyright file="Summary.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace JtlTask.WebApi.Features.Users.GetUserById;

/// <summary>
/// Provides API documentation summary for the GetUserById endpoint.
/// </summary>
internal sealed class GetUserByIdSummary : Summary<Endpoint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByIdSummary"/> class.
    /// </summary>
    public GetUserByIdSummary()
    {
        Summary = "Get user by ID";
        Description = "Retrieves a user by their unique identifier";

        Response<GetUserByIdResponse>(StatusCodes.Status200OK, "User successfully retrieved");

        Response<ProblemDetails>(
            StatusCodes.Status404NotFound,
            "User with the specified ID does not exist"
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
            new(new GetUserByIdRequest(Guid.Parse("11111111-1111-1111-1111-111111111111")))
        );

        Debug.Assert(
            ResponseExamples is not null,
            "ResponseExamples must not be null after base initialization."
        );
        ResponseExamples[StatusCodes.Status200OK] = new GetUserByIdResponse(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "John_Doe"
        );
    }
}
