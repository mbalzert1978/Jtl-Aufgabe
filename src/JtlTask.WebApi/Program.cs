WebApplicationBuilder bld = WebApplication.CreateBuilder(args);
bld.Services.AddAuthenticationJwtBearer(s => s.SigningKey = bld.Configuration["Auth:JwtKey"])
    .AddAuthorization()
    .AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All)
    .SwaggerDocument();

WebApplication app = bld.Build();
app.UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Binding.ReflectionCache.AddFromJtlTaskWebApi();
        c.Errors.UseProblemDetails();
    })
    .UseSwaggerGen();
await app.RunAsync();
