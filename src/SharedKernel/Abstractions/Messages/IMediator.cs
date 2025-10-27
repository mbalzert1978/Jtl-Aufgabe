// <copyright file="IMediator.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel.Abstractions;

namespace Mediator;

/// <summary>
/// Defines a mediator for sending commands and receiving results asynchronously.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a command asynchronously and returns a result.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to send.</typeparam>
    /// <typeparam name="TResult">The type of the result expected from the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation, containing either the result or an error.</returns>
    Task<Result<TResult, IError>> SendAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull;
}
