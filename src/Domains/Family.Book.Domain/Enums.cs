namespace FamilyBook.Domain;

public static class Enums
{
    public enum PhotoStatus
    {
        Uploading,
        Processing,
        Ready,
        Failed
    }

    /// <summary>
    /// HTTP-like status codes for application results
    /// </summary>
    public enum AppStatusCode
    {
        // Success
        Ok = 200,
        Created = 201,
        Accepted = 202,
        NoContent = 204,

        // Client Errors
        BadRequest = 400,
        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        Conflict = 409,
        UnprocessableEntity = 422,

        // Server Errors
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503
    }
}