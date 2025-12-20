using System;
using static FamilyBook.Domain.Enums;

namespace FamilyBook.Domain.Results;

/// <summary>
/// Represents the result of an application operation with error tracking
/// </summary>
public class AppResult
{
    public List<Error> Errors { get; set; } = [];
    public AppStatusCode StatusCode { get; set; }

    public bool IsSuccess => StatusCode >= AppStatusCode.Ok && StatusCode < AppStatusCode.BadRequest;
    public bool HasErrors => Errors.Count > 0;

    // Static factory methods
    public static AppResult Ok() => new()
    {
        StatusCode = AppStatusCode.Ok
    };

    public static AppResult BadRequest(string message) => new()
    {
        StatusCode = AppStatusCode.BadRequest,
        Errors = [new Error { Message = message }]
    };

    public static AppResult BadRequest(List<Error> errors) => new()
    {
        StatusCode = AppStatusCode.BadRequest,
        Errors = errors
    };

    public static AppResult Unauthorized(string message) => new()
    {
        StatusCode = AppStatusCode.Unauthorized,
        Errors = [new Error { Message = message }]
    };

    public static AppResult Forbidden(string message) => new()
    {
        StatusCode = AppStatusCode.Forbidden,
        Errors = [new Error { Message = message }]
    };

    public static AppResult NotFound(string message) => new()
    {
        StatusCode = AppStatusCode.NotFound,
        Errors = [new Error { Message = message }]
    };

    public static AppResult Conflict(string message) => new()
    {
        StatusCode = AppStatusCode.Conflict,
        Errors = [new Error { Message = message }]
    };

    public static AppResult InternalServerError(string message) => new()
    {
        StatusCode = AppStatusCode.InternalServerError,
        Errors = [new Error { Message = message }]
    };

    public static AppResult InternalServerError(Exception ex) => new()
    {
        StatusCode = AppStatusCode.InternalServerError,
        Errors = [new Error { Message = ex.Message }]
    };
}

/// <summary>
/// Represents the result of an application operation with a value
/// </summary>
public sealed class AppResult<T> : AppResult where T : class
{
    public T? Value { get; set; }

    // Static factory methods with value
    public static AppResult<T> Ok(T value) => new()
    {
        StatusCode = AppStatusCode.Ok,
        Value = value
    };

    public static AppResult<T> Created(T value) => new()
    {
        StatusCode = AppStatusCode.Created,
        Value = value
    };

    public new static AppResult<T> BadRequest(string message) => new()
    {
        StatusCode = AppStatusCode.BadRequest,
        Errors = [new Error { Message = message }]
    };

    public new static AppResult<T> BadRequest(List<Error> errors) => new()
    {
        StatusCode = AppStatusCode.BadRequest,
        Errors = errors
    };

    public new static AppResult<T> Unauthorized(string message) => new()
    {
        StatusCode = AppStatusCode.Unauthorized,
        Errors = [new Error { Message = message }]
    };

    public new static AppResult<T> Forbidden(string message) => new()
    {
        StatusCode = AppStatusCode.Forbidden,
        Errors = [new Error { Message = message }]
    };

    public new static AppResult<T> NotFound(string message) => new()
    {
        StatusCode = AppStatusCode.NotFound,
        Errors = [new Error { Message = message }]
    };

    public new static AppResult<T> Conflict(string message) => new()
    {
        StatusCode = AppStatusCode.Conflict,
        Errors = [new Error { Message = message }]
    };

    public new static AppResult<T> InternalServerError(string message) => new()
    {
        StatusCode = AppStatusCode.InternalServerError,
        Errors = [new Error { Message = message }]
    };

    public new static AppResult<T> InternalServerError(Exception ex) => new()
    {
        StatusCode = AppStatusCode.InternalServerError,
        Errors = [new Error { Message = ex.Message }]
    };
}

/// <summary>
/// Represents an error with a message
/// </summary>
public sealed class Error
{
    public string Message { get; set; } = string.Empty;

    public Error() { }

    public Error(string message)
    {
        Message = message;
    }
}
