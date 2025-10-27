using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;

namespace Test.JtlTask.WebApi;

public class App : AppFixture<Program>
{
    protected override ValueTask SetupAsync()
    {
        // place one-time setup for the fixture here
        return ValueTask.CompletedTask;
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

    protected override ValueTask TearDownAsync()
    {
        // do cleanups here
        return ValueTask.CompletedTask;
    }
}
