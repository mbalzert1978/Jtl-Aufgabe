// <copyright file="Mediator.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Monads.Results;
using SharedKernel.Abstractions;

namespace Mediator;

/// <summary>
/// Implements the mediator pattern for handling commands and queries.
/// </summary>
internal sealed class Mediator : IMediator
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="provider">The service provider for resolving handlers and behaviors.</param>
    public Mediator(IServiceProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        _provider = provider;

        Debug.Assert(_provider == provider, "Service provider must be initialized.");
    }

    /// <summary>
    /// Sends a command through the pipeline.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation with the result.</returns>
    public async Task<Result<TResult, IError>> SendAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default
    )
        where TCommand : ICommand<TResult>
        where TResult : notnull
    {
        Debug.Assert(command is not null, "Command must not be null.");
        // csharpier-ignore
        ICommandHandler<TCommand, TResult> handler =
            _provider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        Debug.Assert(handler is not null, "Command handler must be resolved.");
        // csharpier-ignore
        IEnumerable<ICommandPipelineBehavior<TCommand, TResult>> behaviors =
            _provider.GetServices<ICommandPipelineBehavior<TCommand, TResult>>();
        Debug.Assert(behaviors is not null, "Behaviors collection must not be null.");

        Func<Task<Result<TResult, IError>>> pipeline = behaviors
            .Reverse()
            .Aggregate(
                () => handler.HandleAsync(command, cancellationToken),
                (next, behavior) => () => behavior.HandleAsync(command, next, cancellationToken)
            );
        Debug.Assert(pipeline is not null, "Pipeline delegate must be constructed.");

        return await pipeline();
    }
}
