// <copyright file="UserCreatedDomainEvent.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using SharedKernel.Abstractions;

namespace Users.Domain.Models.Users;

/// <summary>
/// Domain event raised when a new user is created.
/// </summary>
/// <param name="UserId">The unique identifier of the created user.</param>
internal sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent;
