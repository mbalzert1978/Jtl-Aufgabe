using SharedKernel.Abstractions;

namespace WorkItems.Application.Adapters;

// TODO: Remove Error after development and replace with specific errors
public sealed record GenericError(string Message, ErrorType Type) : IError;
