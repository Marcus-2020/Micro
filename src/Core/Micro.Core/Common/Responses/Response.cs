using System.Net;
using System.Text.Json.Serialization;

namespace Micro.Core.Common.Responses;

public class Response
{
    private Response(int statusCode, string message, ResponseError[]? errors = null)
    {
        StatusCode = statusCode;
        Message = message ?? "";
        Errors = errors;
    }
    
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

    [JsonPropertyName("message")] 
    public string Message { get; init; }
    
    [JsonPropertyName("Errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseError[]? Errors { get; init; }
    
    public static Response Ok(string message)
    {
        return new((int)HttpStatusCode.OK, message);
    }
    
    public static Response Created(string message)
    {
        return new((int)HttpStatusCode.Created, message);
    }
    
    public static Response Accepted(string message)
    {
        return new((int)HttpStatusCode.Accepted, message);
    }
    
    public static Response BadRequest(string message, ResponseError[]? errors)
    {
        return new((int)HttpStatusCode.Accepted, message, errors);
    }
    
    public static Response UnprocessableEntity(string message, ResponseError[]? errors)
    {
        return new((int)HttpStatusCode.UnprocessableEntity, message, errors);
    }
    
    public static Response InternalServerError(string message, ResponseError[]? errors)
    {
        return new((int)HttpStatusCode.InternalServerError, message, errors);
    }
    
    public static Response Unauthorized()
    {
        return new((int)HttpStatusCode.Unauthorized, string.Empty, default);
    }
    
    public static Response Forbidden()
    {
        return new((int)HttpStatusCode.Forbidden, string.Empty, default);
    }
}

public class Response<T>
{
    private Response(int statusCode, string message, T? data, ResponseError[]? errors = null)
    {
        StatusCode = statusCode;
        Message = message ?? "";
        Data = data;
        Errors = errors;
    }
    
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; }
    
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; init; }
    
    
    [JsonPropertyName("Errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ResponseError[]? Errors { get; init; }
    
    public static Response<T> Ok(T? data, string message)
    {
        return new((int)HttpStatusCode.OK, message, data);
    }
    
    public static Response<T> Created(T? data, string message)
    {
        return new((int)HttpStatusCode.Created, message, data);
    }
    
    public static Response<T> Accepted(T? data, string message)
    {
        return new((int)HttpStatusCode.Accepted, message, data);
    }
    
    public static Response<T> BadRequest(string message, ResponseError[]? errors)
    {
        return new((int)HttpStatusCode.Accepted, message, default, errors);
    }
    
    public static Response<T> UnprocessableEntity(string message, ResponseError[]? errors)
    {
        return new((int)HttpStatusCode.UnprocessableEntity, message, default, errors);
    }
    
    public static Response<T> InternalServerError(string message, ResponseError[]? errors)
    {
        return new((int)HttpStatusCode.InternalServerError, message, default, errors);
    }
    
    public static Response<T> Unauthorized()
    {
        return new((int)HttpStatusCode.Unauthorized, string.Empty, default);
    }
    
    public static Response<T> Forbidden()
    {
        return new((int)HttpStatusCode.Forbidden, string.Empty, default);
    }
}

public record ResponseError(
    [property:JsonPropertyName("code")]
    string Code,
    [property:JsonPropertyName("reason")]
    string Reason);