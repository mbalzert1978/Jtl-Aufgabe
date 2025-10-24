// <copyright file="ICommand.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Abstractions;

/// <summary>
/// Represents a command in the CQRS pattern that produces a response.
/// Commands represent write operations that modify system state.
/// </summary>
/// <typeparam name="TResponse">The type of response produced by executing this command.</typeparam>
public interface ICommand<TResponse>
    where TResponse : notnull;
