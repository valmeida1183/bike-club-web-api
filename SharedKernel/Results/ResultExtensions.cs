namespace BikeClub.SharedKernel.Results;

public static class ResultExtensions
{
    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
    {
        return result.IsSuccess
            ? Result.Success(mapper(result.Value!))
            : Result.Failure<TOut>(result.Error);
    }

    public static async Task<Result<TOut>> Map<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, TOut> mapper)
    {
        var result = await resultTask;
        return result.Map(mapper);
    }

    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value!)
            : Result.Failure<TOut>(result.Error);
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<Result<TOut>>> binder)
    {
        return result.IsSuccess
            ? await binder(result.Value!)
            : Result.Failure<TOut>(result.Error);
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Result<TOut>> binder)
    {
        var result = await resultTask;
        return result.Bind(binder);
    }

    public static async Task<Result<TOut>> Bind<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, Task<Result<TOut>>> binder)
    {
        var result = await resultTask;
        return await result.Bind(binder);
    }

    public static Result<T> Tap<T>(
        this Result<T> result,
        Action<T> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value!);
        }

        return result;
    }

    public static async Task<Result<T>> Tap<T>(
        this Result<T> result,
        Func<T, Task> action)
    {
        if (result.IsSuccess)
        {
            await action(result.Value!);
        }

        return result;
    }

    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value!)
            : onFailure(result.Error);
    }

    public static async Task<TOut> Match<TIn, TOut>(
        this Task<Result<TIn>> resultTask,
        Func<TIn, TOut> onSuccess,
        Func<Error, TOut> onFailure)
    {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        Error error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return predicate(result.Value!)
            ? result
            : Result.Failure<T>(error);
    }

    public static async Task<Result<T>> Ensure<T>(
        this Result<T> result,
        Func<T, Task<bool>> predicate,
        Error error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        return await predicate(result.Value!)
            ? result
            : Result.Failure<T>(error);
    }

    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
            {
                return result;
            }
        }

        return Result.Success();
    }

    public static Result<(T1, T2)> Combine<T1, T2>(
        Result<T1> result1,
        Result<T2> result2)
    {
        if (result1.IsFailure) return Result.Failure<(T1, T2)>(result1.Error);
        if (result2.IsFailure) return Result.Failure<(T1, T2)>(result2.Error);

        return Result.Success((result1.Value!, result2.Value!));
    }

    public static Result<(T1, T2, T3)> Combine<T1, T2, T3>(
        Result<T1> result1,
        Result<T2> result2,
        Result<T3> result3)
    {
        if (result1.IsFailure) return Result.Failure<(T1, T2, T3)>(result1.Error);
        if (result2.IsFailure) return Result.Failure<(T1, T2, T3)>(result2.Error);
        if (result3.IsFailure) return Result.Failure<(T1, T2, T3)>(result3.Error);

        return Result.Success((result1.Value!, result2.Value!, result3.Value!));
    }

    public static T GetValueOrDefault<T>(
        this Result<T> result,
        T defaultValue = default!)
    {
        return result.IsSuccess ? result.Value! : defaultValue;
    }

    public static T GetValueOrDefault<T>(
        this Result<T> result,
        Func<T> defaultFactory)
    {
        return result.IsSuccess ? result.Value! : defaultFactory();
    }
}
