using Microsoft.AspNetCore.Hosting;

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
        // do service configuration here
    }

    protected override ValueTask TearDownAsync()
    {
        // do cleanups here
        return ValueTask.CompletedTask;
    }
}
