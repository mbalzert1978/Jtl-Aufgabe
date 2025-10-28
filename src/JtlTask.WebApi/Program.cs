// <copyright file="Program.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Application;
using SharedKernel;

WebApplicationBuilder bld = WebApplication.CreateBuilder(args);

bld.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

bld.Services.AddAuthenticationJwtBearer(s => s.SigningKey = bld.Configuration["Auth:JwtKey"])
    .AddAuthorization()
    .AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All)
    .SwaggerDocument();

bld.Services.AddMediator();
bld.Services.AddUserApplicationLayer();
bld.Services.AddUserInfrastructureLayer(bld.Configuration);
bld.Services.AddWorkItemApplicationLayer();
bld.Services.AddWorkItemInfrastructureLayer(bld.Configuration);

WebApplication app = bld.Build();
app.UseDefaultExceptionHandler()
    .UseAuthentication()
    .UseAuthorization()
    .UseFastEndpoints(c =>
    {
        c.Binding.ReflectionCache.AddFromJtlTaskWebApi();
        c.Errors.UseProblemDetails();
    })
    .UseSwaggerGen();

await app.Services.EnsureUsersDatabaseCreated();
await app.Services.EnsureWorkItemsDatabaseCreated();
await app.RunAsync();
