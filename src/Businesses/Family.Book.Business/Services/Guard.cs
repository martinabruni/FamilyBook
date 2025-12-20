using System;
using FamilyBook.Domain.Results;
using FamilyBook.Domain.Services;
using Microsoft.Extensions.Logging;
using static FamilyBook.Domain.Enums;

namespace FamilyBook.Business.Services;

/// <summary>
/// Implementation of IGuard that wraps operations in try-catch and aggregates errors
/// </summary>
public sealed class Guard : IGuard
{
    private readonly ILogger<Guard> _logger;

    public Guard(ILogger<Guard> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public AppResult Execute(
        Func<AppResult> operation,
        string? operationName = null)
    {
        var errors = new List<Error>();
        var operationDisplayName = operationName ?? "Synchronous Operation";

        try
        {
            _logger.LogInformation("Starting {OperationName}", operationDisplayName);

            var result = operation();

            if (result.HasErrors)
            {
                errors.AddRange(result.Errors);
                _logger.LogWarning(
                    "{OperationName} completed with {ErrorCount} error(s). Status: {StatusCode}",
                    operationDisplayName,
                    result.Errors.Count,
                    result.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "{OperationName} completed successfully. Status: {StatusCode}",
                    operationDisplayName,
                    result.StatusCode);
            }

            return CreateAggregatedResult(result, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{OperationName} failed with exception: {ErrorMessage}",
                operationDisplayName,
                ex.Message);

            errors.Add(new Error(ex.Message));
            return AppResult.InternalServerError(ex);
        }
    }

    /// <inheritdoc/>
    public AppResult<T> Execute<T>(
        Func<AppResult<T>> operation,
        string? operationName = null) where T : class
    {
        var errors = new List<Error>();
        var operationDisplayName = operationName ?? $"Synchronous Operation<{typeof(T).Name}>";

        try
        {
            _logger.LogInformation("Starting {OperationName}", operationDisplayName);

            var result = operation();

            if (result.HasErrors)
            {
                errors.AddRange(result.Errors);
                _logger.LogWarning(
                    "{OperationName} completed with {ErrorCount} error(s). Status: {StatusCode}",
                    operationDisplayName,
                    result.Errors.Count,
                    result.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "{OperationName} completed successfully. Status: {StatusCode}",
                    operationDisplayName,
                    result.StatusCode);
            }

            return CreateAggregatedResult(result, errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{OperationName} failed with exception: {ErrorMessage}",
                operationDisplayName,
                ex.Message);

            errors.Add(new Error(ex.Message));
            return AppResult<T>.InternalServerError(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<AppResult> ExecuteAsync(
        Func<Task<AppResult>> operation,
        string? operationName = null,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<Error>();
        var operationDisplayName = operationName ?? "Asynchronous Operation";

        try
        {
            _logger.LogInformation("Starting {OperationName}", operationDisplayName);

            cancellationToken.ThrowIfCancellationRequested();

            var result = await operation();

            if (result.HasErrors)
            {
                errors.AddRange(result.Errors);
                _logger.LogWarning(
                    "{OperationName} completed with {ErrorCount} error(s). Status: {StatusCode}",
                    operationDisplayName,
                    result.Errors.Count,
                    result.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "{OperationName} completed successfully. Status: {StatusCode}",
                    operationDisplayName,
                    result.StatusCode);
            }

            return CreateAggregatedResult(result, errors);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("{OperationName} was cancelled", operationDisplayName);
            return AppResult.BadRequest("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{OperationName} failed with exception: {ErrorMessage}",
                operationDisplayName,
                ex.Message);

            errors.Add(new Error(ex.Message));
            return AppResult.InternalServerError(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<AppResult<T>> ExecuteAsync<T>(
        Func<Task<AppResult<T>>> operation,
        string? operationName = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var errors = new List<Error>();
        var operationDisplayName = operationName ?? $"Asynchronous Operation<{typeof(T).Name}>";

        try
        {
            _logger.LogInformation("Starting {OperationName}", operationDisplayName);

            cancellationToken.ThrowIfCancellationRequested();

            var result = await operation();

            if (result.HasErrors)
            {
                errors.AddRange(result.Errors);
                _logger.LogWarning(
                    "{OperationName} completed with {ErrorCount} error(s). Status: {StatusCode}",
                    operationDisplayName,
                    result.Errors.Count,
                    result.StatusCode);
            }
            else
            {
                _logger.LogInformation(
                    "{OperationName} completed successfully. Status: {StatusCode}",
                    operationDisplayName,
                    result.StatusCode);
            }

            return CreateAggregatedResult(result, errors);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("{OperationName} was cancelled", operationDisplayName);
            return AppResult<T>.BadRequest("Operation was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "{OperationName} failed with exception: {ErrorMessage}",
                operationDisplayName,
                ex.Message);

            errors.Add(new Error(ex.Message));
            return AppResult<T>.InternalServerError(ex);
        }
    }

    /// <inheritdoc/>
    public AppResult ExecuteMany(
        IEnumerable<Func<AppResult>> operations,
        string? operationName = null)
    {
        var errors = new List<Error>();
        var operationDisplayName = operationName ?? "Multiple Synchronous Operations";
        var operationsList = operations.ToList();

        _logger.LogInformation(
            "Starting {OperationName} with {OperationCount} operation(s)",
            operationDisplayName,
            operationsList.Count);

        var worstStatusCode = AppStatusCode.Ok;

        foreach (var (operation, index) in operationsList.Select((op, i) => (op, i)))
        {
            try
            {
                var result = operation();

                if (result.HasErrors)
                {
                    errors.AddRange(result.Errors);
                    _logger.LogWarning(
                        "Operation #{Index} in {OperationName} completed with errors. Status: {StatusCode}",
                        index + 1,
                        operationDisplayName,
                        result.StatusCode);
                }

                // Track the worst status code
                if ((int)result.StatusCode > (int)worstStatusCode)
                {
                    worstStatusCode = result.StatusCode;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Operation #{Index} in {OperationName} failed with exception",
                    index + 1,
                    operationDisplayName);

                errors.Add(new Error($"Operation #{index + 1}: {ex.Message}"));
                worstStatusCode = AppStatusCode.InternalServerError;
            }
        }

        var finalResult = errors.Count > 0
            ? new AppResult { StatusCode = worstStatusCode, Errors = errors }
            : AppResult.Ok();

        _logger.LogInformation(
            "{OperationName} completed with {ErrorCount} total error(s). Final Status: {StatusCode}",
            operationDisplayName,
            errors.Count,
            finalResult.StatusCode);

        return finalResult;
    }

    /// <inheritdoc/>
    public async Task<AppResult> ExecuteManyAsync(
        IEnumerable<Func<Task<AppResult>>> operations,
        string? operationName = null,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<Error>();
        var operationDisplayName = operationName ?? "Multiple Asynchronous Operations";
        var operationsList = operations.ToList();

        _logger.LogInformation(
            "Starting {OperationName} with {OperationCount} operation(s)",
            operationDisplayName,
            operationsList.Count);

        var worstStatusCode = AppStatusCode.Ok;

        foreach (var (operation, index) in operationsList.Select((op, i) => (op, i)))
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await operation();

                if (result.HasErrors)
                {
                    errors.AddRange(result.Errors);
                    _logger.LogWarning(
                        "Operation #{Index} in {OperationName} completed with errors. Status: {StatusCode}",
                        index + 1,
                        operationDisplayName,
                        result.StatusCode);
                }

                // Track the worst status code
                if ((int)result.StatusCode > (int)worstStatusCode)
                {
                    worstStatusCode = result.StatusCode;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Operation #{Index} in {OperationName} was cancelled",
                    index + 1,
                    operationDisplayName);

                errors.Add(new Error($"Operation #{index + 1} was cancelled"));
                worstStatusCode = AppStatusCode.BadRequest;
                break; // Stop processing on cancellation
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Operation #{Index} in {OperationName} failed with exception",
                    index + 1,
                    operationDisplayName);

                errors.Add(new Error($"Operation #{index + 1}: {ex.Message}"));
                worstStatusCode = AppStatusCode.InternalServerError;
            }
        }

        var finalResult = errors.Count > 0
            ? new AppResult { StatusCode = worstStatusCode, Errors = errors }
            : AppResult.Ok();

        _logger.LogInformation(
            "{OperationName} completed with {ErrorCount} total error(s). Final Status: {StatusCode}",
            operationDisplayName,
            errors.Count,
            finalResult.StatusCode);

        return finalResult;
    }

    /// <summary>
    /// Creates an aggregated result, preserving the original result if no additional errors
    /// </summary>
    private static AppResult CreateAggregatedResult(AppResult originalResult, List<Error> aggregatedErrors)
    {
        if (aggregatedErrors.Count == 0)
        {
            return originalResult;
        }

        // If we have aggregated errors, return a result with all errors
        return new AppResult
        {
            StatusCode = originalResult.StatusCode,
            Errors = aggregatedErrors
        };
    }

    /// <summary>
    /// Creates an aggregated result for generic type
    /// </summary>
    private static AppResult<T> CreateAggregatedResult<T>(AppResult<T> originalResult, List<Error> aggregatedErrors)
        where T : class
    {
        if (aggregatedErrors.Count == 0)
        {
            return originalResult;
        }

        // If we have aggregated errors, return a result with all errors
        return new AppResult<T>
        {
            StatusCode = originalResult.StatusCode,
            Errors = aggregatedErrors,
            Value = originalResult.Value
        };
    }
}
