// <copyright file="UserExistenceServiceTests.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Monads.Results;
using SharedKernel.Abstractions;
using Shouldly;
using Users.Application.Adapters.ExistenceServiceTestApi;

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
        var userId = Guid.NewGuid();
        IUserExistenceService existenceService = UserExistenceServiceTestApiBuilder
            .New()
            .WithExistingUser(userId, "existingUser")
            .Build();

        // Act
        Result<Unit, IError> exists = await existenceService.VerifyUserExistsAsync(
            userId,
            TestContext.Current.CancellationToken
        );

        // Assert
        exists.IsOk.ShouldBeTrue();
    }

    [Fact]
    public async Task UserExistenceService_ExistsAsync_WhenUserDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        IUserExistenceService existenceService = UserExistenceServiceTestApiBuilder.New().Build();

        // Act
        Result<Unit, IError> exists = await existenceService.VerifyUserExistsAsync(
            userId,
            TestContext.Current.CancellationToken
        );

        // Assert
        exists.IsErr.ShouldBeTrue();
    }
}
