// <copyright file="ICommandPipelineBehavior.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel.Abstractions;

namespace Mediator;

/// <summary>
/// Defines a behavior that can be applied to a pipeline to handle cross-cutting concerns.
/// </summary>
/// <typeparam name="TRequest">The type of the input to the pipeline.</typeparam>
/// <typeparam name="TResponse">The type of the output from the pipeline.</typeparam>
public interface ICommandPipelineBehavior<in TRequest, TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the pipeline execution, allowing for pre- and post-processing logic.
    /// </summary>
    /// <param name="request">The input to the pipeline.</param>
    /// <param name="delegate">A delegate representing the next behavior or handler in the pipeline.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the result of the pipeline execution.</returns>
    Task<Result<TResponse, IError>> HandleAsync(
        TRequest request,
        Func<Task<Result<TResponse, IError>>> @delegate,
        CancellationToken cancellationToken = default
    );
}
