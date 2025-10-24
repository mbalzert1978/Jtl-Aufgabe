// <copyright file="Program.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using Application;
using Microsoft.AspNetCore.Server.Kestrel.Core;

WebApplicationBuilder bld = WebApplication.CreateBuilder(args);

bld.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

bld.Services.AddAuthenticationJwtBearer(s => s.SigningKey = bld.Configuration["Auth:JwtKey"])
    .AddAuthorization()
    .AddFastEndpoints(o => o.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All)
    .SwaggerDocument();

bld.Services.AddUserApplicationLayer();
bld.Services.AddUserInfrastructureLayer();

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
