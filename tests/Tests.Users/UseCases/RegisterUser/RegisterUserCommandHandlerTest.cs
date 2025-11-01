using Microsoft.Extensions.Time.Testing;
using Monads.Results;
using SharedKernel.Abstractions;
using Shouldly;
using Users.Application.Adapters;
using Users.Application.Adapters.ExistenceServiceTestApi;
using Users.Application.UseCases.RegisterUser;

namespace Tests.Users.UseCases.RegisterUser;

public sealed class RegisterUserCommandHandlerTest
{
    /// <summary>
    /// Initializes a <see cref="RegisterUserCommandHandler"/> with default dependencies for testing.
    /// </summary>
    /// <param name="now">The current time to use for the time provider. If null, a default time will be used.</param>
    /// <returns>A configured instance of <see cref="RegisterUserCommandHandler"/>.</returns>
    private static RegisterUserCommandHandler InitializeRegisterUserHandler(
        DateTimeOffset? now = null
    ) =>
        new(
            new UserRepository(new InMemoryUsersDatabase()),
            new FakeTimeProvider(now ?? new(2025, 10, 30, 12, 0, 0, TimeSpan.Zero))
        );

    [Fact]
    public async Task RegisterUserCommandHandler_WhenUserDoesNotExist_ShouldRegisterUserSuccessfully()
    {
        // Arrange
        RegisterUserCommandHandler handler = InitializeRegisterUserHandler();
        string userName = "test_user";

        // Act
        Result<UserEntity, IError> result = await handler.HandleAsync(
            new RegisterUserCommand(userName),
            TestContext.Current.CancellationToken
        );

        // Assert
        result
            .IsOkAnd(user =>
            {
                user.UserId.ShouldNotBe(Guid.Empty, "UserId should be assigned.");
                user.Username.ShouldBe(userName, "Username should match the input.");
                return true;
            })
            .ShouldBeTrue();
    }
}
