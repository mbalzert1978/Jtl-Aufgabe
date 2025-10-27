// <copyright file="LoggingBehavior.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mediator;
using Microsoft.Extensions.Logging;
using Monads.Results;
using SharedKernel.Abstractions;

namespace SharedKernel.Behaviors;

/// <summary>
/// Pipeline behavior that logs command execution details including timing and outcomes.
/// </summary>
/// <typeparam name="TRequest">The command request type.</typeparam>
/// <typeparam name="TResponse">The command response type.</typeparam>
internal sealed class LoggingBehavior<TRequest, TResponse>
    : ICommandPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : notnull
{
    private readonly ILoggerFactory _factory;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="factory">The logger factory for creating loggers.</param>
    /// <exception cref="ArgumentNullException">Thrown when factory is null.</exception>
    public LoggingBehavior(ILoggerFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;

        Debug.Assert(_factory == factory, "LoggerFactory must be initialized.");
    }

    /// <summary>
    /// Handles the command execution with logging.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="delegate">The delegate to execute the command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result containing either the response or an error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request or delegate is null.</exception>
    public async Task<Result<TResponse, IError>> HandleAsync(
        TRequest request,
        Func<Task<Result<TResponse, IError>>> @delegate,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(@delegate);

        string requestName = typeof(TRequest).Name;

        ILogger logger = _factory.CreateLogger<TRequest>();
        using IDisposable? scope = logger.BeginScope("Handling {RequestType}", requestName);
        Debug.Assert(scope is not null, "Logger scope must be created.");

        logger.LogDebug(1000, "Started handling {RequestType}", requestName);

        var sw = Stopwatch.StartNew();
        try
        {
            Result<TResponse, IError> result = await @delegate();
            sw.Stop();

            if (result.IsOk)
            {
                logger.LogDebug(
                    1001,
                    "Success handling {RequestType} in {ElapsedMilliseconds} ms",
                    requestName,
                    sw.ElapsedMilliseconds
                );
            }
            else
            {
                logger.LogDebug(
                    1002,
                    "Failure handling {RequestType} in {ElapsedMilliseconds} ms",
                    requestName,
                    sw.ElapsedMilliseconds
                );
            }

            Debug.Assert(result is not null, "Result must not be null after successful execution.");

            return result;
        }
        // S2139: Logging behavior should preserve the original exception and stack trace
        // for debugging purposes. Wrapping would obscure the actual error source.
#pragma warning disable S2139
        catch (Exception exc)
#pragma warning restore S2139
        {
            sw.Stop();
            logger.LogError(
                1005,
                exc,
                "Exception handling {RequestType} in {ElapsedMilliseconds} ms",
                requestName,
                sw.ElapsedMilliseconds
            );

            throw;
        }
    }
}
