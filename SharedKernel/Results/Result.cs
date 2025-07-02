namespace SharedKernel.Results;

public class Result
{
    public bool IsSuccess { get; init; }
    public string? Error { get; init; }

    public static Result Success(string? message = null) => new() { IsSuccess = true, Error = message };
    public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
}