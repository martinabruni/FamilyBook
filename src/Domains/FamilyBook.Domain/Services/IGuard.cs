using System;
using FamilyBook.Domain.Results;

namespace FamilyBook.Domain.Services;

/// <summary>
/// Interface for guarding operations with try-catch wrapper and error aggregation
/// </summary>
public interface IGuard
{
    /// <summary>
    /// Executes a synchronous operation that returns AppResult
    /// </summary>
    AppResult Execute(
        Func<AppResult> operation,
        string? operationName = null);

    /// <summary>
    /// Executes a synchronous operation that returns AppResult<T>
    /// </summary>
    AppResult<T> Execute<T>(
        Func<AppResult<T>> operation,
        string? operationName = null) where T : class;

    /// <summary>
    /// Executes an asynchronous operation that returns AppResult
    /// </summary>
    Task<AppResult> ExecuteAsync(
        Func<Task<AppResult>> operation,
        string? operationName = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an asynchronous operation that returns AppResult<T>
    /// </summary>
    Task<AppResult<T>> ExecuteAsync<T>(
        Func<Task<AppResult<T>>> operation,
        string? operationName = null,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Executes multiple synchronous operations and aggregates their results
    /// </summary>
    AppResult ExecuteMany(
        IEnumerable<Func<AppResult>> operations,
        string? operationName = null);

    /// <summary>
    /// Executes multiple asynchronous operations and aggregates their results
    /// </summary>
    Task<AppResult> ExecuteManyAsync(
        IEnumerable<Func<Task<AppResult>>> operations,
        string? operationName = null,
        CancellationToken cancellationToken = default);
}
