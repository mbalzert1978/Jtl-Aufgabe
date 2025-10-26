using SharedKernel.Models.Common;

// <copyright file="IAggregateRoot.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Abstractions;

/// <summary>
/// Marker interface that identifies an entity as an aggregate root in Domain-Driven Design.
/// Aggregate roots are the entry points for accessing a cluster of related entities and value objects,
/// ensuring consistency boundaries within the domain model.
/// </summary>
internal interface IAggregateRoot;
