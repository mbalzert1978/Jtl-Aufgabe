using System.Diagnostics;
using Monads.Results;
using SharedKernel.Abstractions;
using SharedKernel.Models.Common;
using Users.Domain.Abstractions;
using Users.Domain.Models.Users;
using static Monads.Results.ResultFactory;
using static SharedKernel.Models.Common.DomainErrorFactory;

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

            await _database.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exc)
        {
            DomainError error = DatabaseError(exc);
            return Failure<Unit, DomainError>(error);
        }
        return Success<Unit, DomainError>(default);
    }

    public async Task<Result<User, DomainError>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    )
    {
        Debug.Assert(id != Guid.Empty, "User ID must not be empty.");

        try
        {
            UserEntity? entity = _database.Query<UserEntity>().FirstOrDefault(u => u.UserId == id);

            if (entity is null)
                return Failure<User, DomainError>(NotFound(id, typeof(UserEntity)));

            Debug.Assert(entity.UserId == id, "Retrieved entity ID does not match requested ID.");

            var user = User.Rehydrate(entity.UserId, entity.Username);

            Debug.Assert(user.Id == id, "Rehydrated user ID does not match.");

            return Success<User, DomainError>(user);
        }
        catch (Exception exc)
        {
            DomainError error = DatabaseError(exc);
            return Failure<User, DomainError>(error);
        }
    }
}
