using BikeClub.SharedKernel.Results;

namespace BikeClub.SharedKernel.Http;

public static class ResultExtensions
{
    public static IResult ToIResult(this Result result, Func<Result, IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess is not null ? onSuccess(result) : Microsoft.AspNetCore.Http.Results.NoContent();
        }

        return MapFailure(result, result.Error);
    }

    public static IResult ToIResult<T>(this Result<T> result, Func<Result<T>, IResult>? onSuccess = null)
    {
        if (result.IsSuccess)
        {
            return onSuccess is not null
                ? onSuccess(result)
                : Microsoft.AspNetCore.Http.Results.Ok(result);
        }

        return MapFailure(result, result.Error);
    }

    private static IResult MapFailure(object body, Error error) => error.Type switch
    {
        ErrorType.Validation => Microsoft.AspNetCore.Http.Results.BadRequest(body),
        ErrorType.NotFound => Microsoft.AspNetCore.Http.Results.NotFound(body),
        ErrorType.Conflict => Microsoft.AspNetCore.Http.Results.Conflict(body),
        ErrorType.Unauthorized => Microsoft.AspNetCore.Http.Results.Unauthorized(),
        ErrorType.Forbidden => Microsoft.AspNetCore.Http.Results.Forbid(),
        _ => Microsoft.AspNetCore.Http.Results.BadRequest(body)
    };
}
