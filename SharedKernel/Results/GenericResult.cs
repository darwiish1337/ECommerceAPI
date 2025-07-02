namespace SharedKernel.Results;

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static new Result<T> Failure(string error) =>
        new() { IsSuccess = false, Error = error };
}