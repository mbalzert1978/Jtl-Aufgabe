// <copyright file="UserExistenceServiceTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Shouldly;
using Users.Application.Adapters;
using Users.Application.Adapters.TestFramework;
using Users.Domain.Abstractions;

namespace Tests.Users.Domain;

/// <summary>
/// Tests for the <see cref="IUserExistenceService"/> implementation.
/// </summary>
/// <remarks>
/// These tests verify the domain service for checking user existence
/// using the TestFramework pattern with InMemoryUsersDatabase.
/// </remarks>
public sealed class UserExistenceServiceTests
{
    /// <summary>
    /// Verifies that ExistsAsync returns true when a user exists in the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task UserExistenceService_ExistsAsync_WhenUserExists_ShouldReturnTrue()
    {
        // Arrange
        Guid userId = Guid.NewGuid();
        InMemoryUsersDatabase database = new();
        UserEntity userEntity = new(userId, "testuser");
        await database.AddAsync(userEntity, CancellationToken.None);

        // Note: IUserExistenceService doesn't exist yet - this will fail to compile
        // This is expected in the RED phase of TDD
        IUserExistenceService service = new UserExistenceService(database);

        // Act
        bool exists = await service.ExistsAsync(userId, CancellationToken.None);

        // Assert
        exists.ShouldBeTrue();
    }
}
