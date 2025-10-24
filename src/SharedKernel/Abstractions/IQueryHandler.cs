// <copyright file="IQueryHandler.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel;
using SharedKernel.Abstractions;

namespace Application.Abstractions.Messaging;

/// <summary>
/// Defines a handler for processing queries in the CQRS pattern.
/// Query handlers are responsible for executing read operations that retrieve data without modifying system state.
/// </summary>
/// <typeparam name="TQuery">The type of query to handle.</typeparam>
/// <typeparam name="TResponse">The type of response produced by the query handler.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the specified query asynchronously.
    /// </summary>
    /// <param name="query">The query to handle.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result containing either the query response or an error.</returns>
    Task<Result<TResponse, IError>> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
