using System.Diagnostics;
using Monads.Results;
using SharedKernel.Abstractions;
using Users.Domain;
using Users.Domain.Abstractions;
using Users.Domain.Models.Users;
using static Monads.Results.ResultFactory;
using static Users.Domain.DomainErrorFactory;

namespace Users.Application.Adapters;

internal sealed class UserRepository : IUserRepository
{
    private readonly IDatabase _database;

    public UserRepository(IDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _database = database;

        Debug.Assert(_database == database, "Database instance was not set correctly.");
    }

    public async Task<Result<Unit, DomainError>> AddAsync(
        User user,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            UserEntity entity = new(user.Id, user.Username.Value);
            Debug.Assert(entity.UserId == user.Id, "UserEntity ID does not match");

            await _database.AddAsync(entity, cancellationToken);
            return Success<Unit, DomainError>(default);
        }
        catch (Exception exc)
        {
            DomainError error = DatabaseError(exc);
            return Failure<Unit, DomainError>(error);
        }
    }

    public async Task<Result<User, DomainError>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            return (await _database.GetAsync<UserEntity>(cancellationToken)).FirstOrDefault(u =>
                u.UserId == id
            ) switch
            {
                UserEntity e => Success<User, DomainError>(
                    UserFactory.Rehydrate(e.UserId, e.Username)
                ),
                _ => Failure<User, DomainError>(UserNotFound(id)),
            };
        }
        catch (Exception exc)
        {
            DomainError error = DatabaseError(exc);
            return Failure<User, DomainError>(error);
        }
    }
}
