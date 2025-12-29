namespace SharedKernel.Application;

/// <summary>
/// Result pattern - pra não ficar jogando exceptions pra todo lado
/// Inspirado no Railway Oriented Programming, mas sem complicar demais
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        // Aqui a gente garante que não vai ter resultado inconsistente
        if (isSuccess && !string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Success result cannot have error message");
        
        if (!isSuccess && string.IsNullOrEmpty(error))
            throw new InvalidOperationException("Failure result must have error message");

        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);

    // Implicit operators pra facilitar o uso
    public static implicit operator Result(string error) => Failure(error);
}

/// <summary>
/// Result com valor de retorno - pra quando precisa devolver algo junto
/// </summary>
public class Result<T> : Result
{
    public T? Value { get; }

    protected Result(bool isSuccess, T? value, string error) : base(isSuccess, error)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, string.Empty);
    public static new Result<T> Failure(string error) => new(false, default, error);

    // Implicit operators
    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(string error) => Failure(error);
}