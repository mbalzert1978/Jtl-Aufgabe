// <copyright file="Summary.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace JtlTask.WebApi.Features.Users.RegisterUser;

/// <summary>
/// Defines the OpenAPI summary for the register user endpoint.
/// </summary>
internal sealed class RegisterUserSummary : Summary<Endpoint>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserSummary"/> class.
    /// </summary>
    public RegisterUserSummary()
    {
        Summary = "Register a new user";
        Description = "Registers a new user in the system";

        Response<RegisterUserResponse>(
            StatusCodes.Status201Created,
            "User successfully registered"
        );

        Response<ProblemDetails>(StatusCodes.Status400BadRequest, "The request was invalid.");

        Response<InternalErrorResponse>(
            StatusCodes.Status500InternalServerError,
            "An internal server error occurred."
        );

        Debug.Assert(RequestExamples is not null, "RequestExamples must not be null.");
        RequestExamples.Add(new(new RegisterUserRequest("John_Doe")));

        Debug.Assert(ResponseExamples is not null, "ResponseExamples must not be null.");
        ResponseExamples[StatusCodes.Status201Created] = new RegisterUserResponse(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "John_Doe"
        );
    }
}
