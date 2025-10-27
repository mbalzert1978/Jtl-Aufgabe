// <copyright file="RegisterUserHandler.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Mediator;
using Monads.Results;
using Monads.Results.Extensions.Async;
using SharedKernel.Abstractions;
using SharedKernel.Models;
using Users.Application.Adapters;
using Users.Domain.Abstractions;
using Users.Domain.Models.Users;

namespace Users.Application.UseCases.RegisterUser;

/// <summary>
/// Handles the registration of new users in the system.
/// </summary>
internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, UserEntity>
{
    private readonly IUserRepository _userRepository;
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The repository for user persistence operations.</param>
    /// <param name="timeProvider">The time provider for event timestamps.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="userRepository"/> is null.</exception>
    public RegisterUserCommandHandler(IUserRepository userRepository, TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        ArgumentNullException.ThrowIfNull(timeProvider);

        _userRepository = userRepository;
        _timeProvider = timeProvider;

        Debug.Assert(
            _userRepository == userRepository,
            "UserRepository instance was not set correctly."
        );
        Debug.Assert(_timeProvider == timeProvider, "TimeProvider instance was not set correctly.");
    }

    /// <summary>
    /// Handles the user registration command by creating and persisting a new user.
    /// </summary>
    /// <param name="command">The registration command containing user details.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>A result containing the created user entity or an error.</returns>
    public async Task<Result<UserEntity, IError>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken
    )
    {
        Debug.Assert(command is not null, "Command must not be null.");

        return await User.Create(command.Username, _timeProvider)
            .BindAsync(user =>
                _userRepository
                    .AddAsync(user, cancellationToken)
                    .MapAsync(_ => new UserEntity(
                        user.Id,
                        user.Username.Value,
                        user.Tasks.Select(t => new UserTaskReference(t))
                    ))
            )
            .MapErrAsync(ApplicationErrorFactory.FromDomainError);
    }
}
