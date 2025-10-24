// <copyright file="ICommandHandler.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel;

namespace SharedKernel.Abstractions;

/// <summary>
/// Defines a handler for processing commands in the CQRS pattern.
/// Command handlers are responsible for executing business logic that modifies system state.
/// </summary>
/// <typeparam name="TCommand">The type of command to handle.</typeparam>
/// <typeparam name="TResponse">The type of response produced by the command handler.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result containing either the command response or an error.</returns>
    Task<Result<TResponse, IError>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken
    );
}
