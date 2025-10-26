// <copyright file="IDomainEvent.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Abstractions;

/// <summary>
/// Marker interface that identifies a domain event in Domain-Driven Design.
/// Domain events represent significant occurrences within the domain that domain experts care about.
/// They are used to capture and communicate state changes within aggregates.
/// </summary>
internal interface IDomainEvent;
