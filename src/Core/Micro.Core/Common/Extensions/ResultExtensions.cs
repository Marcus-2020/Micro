using FluentResults;

namespace Micro.Core.Common.Extensions;

public static class ResultExtensions
{
    public static Exception? GetFirstException(this Result result)
    {
        if (result.IsSuccess || result.Errors.Count == 0)
            return null;
        return result.Errors[0].Reasons.OfType<ExceptionalError>()
            .FirstOrDefault()?.Exception;
    }
    
    public static Exception? GetFirstException<T>(this Result<T> result)
    {
        if (result.IsSuccess || result.Errors.Count == 0)
            return null;
        return result.Errors[0].Reasons.OfType<ExceptionalError>()
            .FirstOrDefault()?.Exception;
    }
}