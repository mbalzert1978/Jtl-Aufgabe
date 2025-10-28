// <copyright file="UserExistenceServiceTestApiBuilder.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using SharedKernel.Abstractions;
using Users.Domain.Abstractions;

namespace Users.Application.Adapters.ExistenceServiceTestApi;

/// <summary>
/// Builder for creating configured instances of <see cref="IUserExistenceService"/> for testing.
/// </summary>
internal sealed class UserExistenceServiceTestApiBuilder
{
    private readonly IUsersDatabase _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserExistenceServiceTestApiBuilder"/> class.
    /// </summary>
    /// <param name="repository">The user repository to use.</param>
    /// <exception cref="ArgumentNullException">Thrown when repository is null.</exception>
    private UserExistenceServiceTestApiBuilder(IUsersDatabase repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        _repository = repository;
        Debug.Assert(_repository == repository, "Repository should be assigned correctly");
    }

    /// <summary>
    /// Creates a new builder instance.
    /// </summary>
    /// <param name="repository">The user repository to use.</param>
    /// <returns>A new instance of the builder.</returns>
    public static UserExistenceServiceTestApiBuilder New(IUsersDatabase? repository = null) =>
        new(repository ?? new InMemoryUsersDatabase());

    /// <summary>
    /// Configures the builder to include an existing user.
    /// </summary>
    /// <param name="userId">The ID of the existing user. If null, a new GUID will be generated.</param>
    /// <param name="username">The username of the existing user.</param>
    /// <returns>The builder instance for method chaining.</returns>
    public UserExistenceServiceTestApiBuilder WithExistingUser(
        Guid userId,
        string username = "existingUser"
    )
    {
        UserEntity userEntity = new(userId, username);
        _repository.AddAsync(userEntity).GetAwaiter();
        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="IUserExistenceService"/> instance.
    /// </summary>
    /// <returns>A configured user existence service.</returns>
    public IUserExistenceService Build()
    {
        Debug.Assert(_repository is not null, "Repository should be assigned correctly");

        IUserService userService = new FakeUserService(_repository);
        UserExistenceService existenceService = new(userService);
        Debug.Assert(existenceService is not null, "ExistenceService should be created correctly");

        return existenceService;
    }
}
