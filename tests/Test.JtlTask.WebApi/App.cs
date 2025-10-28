using JtlTask.WebApi.Features.Users.RegisterUser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Users.Infrastructure.Persistence;
using WorkItems.Infrastructure.Persistence;

namespace Test.JtlTask.WebApi;

public class App : AppFixture<Program>
{
    public async Task<TestUser> CreateTestUser(string username = "John_Doe")
    {
        using HttpClient client = CreateClient();
        (HttpResponseMessage resp, RegisterUserResponse result) = await client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(new(username));

        resp.EnsureSuccessStatusCode();

        return new(result.UserId, result.Username);
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        s.AddLogging(opt => opt.AddConsole());

        s.RemoveAll<TimeProvider>();
        FakeTimeProvider provider = new(new(2025, 10, 27, 12, 0, 0, TimeSpan.Zero));
        s.AddSingleton<TimeProvider>(provider);
    }

    protected override async ValueTask TearDownAsync() { }

    public async Task CleanDatabaseAsync()
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        UsersDbContext uContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        WorkItemsDbContext wContext =
            scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>();

        await wContext.Database.ExecuteSqlRawAsync("DELETE FROM WorkItems");
        await uContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");
    }
}

public sealed record TestUser(Guid UserId, string Username);
