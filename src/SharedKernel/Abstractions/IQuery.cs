// <copyright file="IQuery.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Abstractions;

/// <summary>
/// Represents a query in the CQRS pattern that produces a response.
/// Queries represent read operations that retrieve data without modifying system state.
/// </summary>
/// <typeparam name="TResponse">The type of response produced by executing this query.</typeparam>
public interface IQuery<TResponse>
    where TResponse : notnull;
