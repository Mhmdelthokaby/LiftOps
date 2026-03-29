namespace Collins_BackEnd.Application.Common;

public class Result
{
    public bool Succeeded { get; protected set; }
    public string[] Errors { get; protected set; } = Array.Empty<string>();

    public static Result Success() => new Result { Succeeded = true };
    public static Result Failure(params string[] errors) => new Result { Succeeded = false, Errors = errors };
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    public static Result<T> Success(T data) => new Result<T> { Succeeded = true, Data = data };
    public new static Result<T> Failure(params string[] errors) => new Result<T> { Succeeded = false, Errors = errors };
}
