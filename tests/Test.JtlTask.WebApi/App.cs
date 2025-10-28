using JtlTask.WebApi.Features.Users.RegisterUser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Users.Infrastructure.Persistence;
using WorkItems.Infrastructure.Persistence;

namespace Test.JtlTask.WebApi;

public class App : AppFixture<Program>
{
    public TestUser TestUser { get; private set; } = null!;
    public TestUser TestUser2 { get; private set; } = null!;

    protected override async ValueTask SetupAsync()
    {
        RegisterUserResponse result = await CreateTestUser("John_Doe");
        TestUser = new(result.UserId, result.Username);
        RegisterUserResponse result2 = await CreateTestUser("Jane_Smith");
        TestUser2 = new(result2.UserId, result2.Username);
    }

    private async Task<RegisterUserResponse> CreateTestUser(string username = "John_Doe")
    {
        using HttpClient client = CreateClient();
        (HttpResponseMessage resp, RegisterUserResponse result) = await client.POSTAsync<
            Endpoint,
            RegisterUserRequest,
            RegisterUserResponse
        >(new(username));
        resp.EnsureSuccessStatusCode();
        return result;
    }

    protected override void ConfigureApp(IWebHostBuilder a)
    {
        // do host builder configuration here
    }

    protected override void ConfigureServices(IServiceCollection s)
    {
        s.AddLogging(opt => opt.AddConsole());

        s.RemoveAll<TimeProvider>();
        FakeTimeProvider provider = new(new(2025, 10, 27, 12, 0, 0, TimeSpan.Zero));
        s.AddSingleton<TimeProvider>(provider);
    }

    protected override async ValueTask TearDownAsync()
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        UsersDbContext uContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();
        WorkItemsDbContext wContext = scope.ServiceProvider.GetRequiredService<WorkItemsDbContext>();

        await wContext.Database.ExecuteSqlRawAsync("DELETE FROM WorkItems");
        await uContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");
    }
}

public sealed record TestUser(Guid UserId, string Username);
