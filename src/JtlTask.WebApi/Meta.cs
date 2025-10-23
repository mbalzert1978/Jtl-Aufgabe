global using FastEndpoints;
global using FastEndpoints.Security;
global using FastEndpoints.Swagger;
global using JtlTask.WebApi;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test.JtlTask.WebApi")]

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Style",
    "S3903:Types should be defined in named namespaces",
    Justification = "Required for WebApplicationFactory to locate the entry point of the tested project."
)]
public partial class Program;
