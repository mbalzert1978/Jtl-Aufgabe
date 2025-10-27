// <copyright file="IError.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Abstractions;

/// <summary>
/// Represents the type of error that occurred.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Indicates that a requested resource was not found.
    /// </summary>
    NotFound,

    /// <summary>
    /// Indicates that validation of input data failed.
    /// </summary>
    Validation,

    /// <summary>
    /// Indicates an internal server error.
    /// </summary>
    Internal,
}

/// <summary>
/// Represents an error that occurred during application execution.
/// </summary>
public interface IError
{
    /// <summary>
    /// Gets the type of the error.
    /// </summary>
    ErrorType ErrorType { get; }

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    string Message { get; }
}
