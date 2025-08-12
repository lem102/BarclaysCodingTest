namespace BarclaysCodingTest.Utilities;

public record Result
{
    public bool IsSuccess { get; }
    
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    
    public static Result Failure(Error error) =>
        new(false, error ?? throw new ArgumentNullException(nameof(error)));

    public static implicit operator Result(Error error) => Failure(error);
}

public record Result<T> : Result
{
    public T? Value { get; }

    private Result(T value) : base(true, null) {
        Value = value;
    }
    
    private Result(Error error) : base(false, error) { }

    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Error error) => new(error);
}

public record Error(string Id, ErrorType Type, string Description);

public static class Errors
{
    public static Error UserRequiredDataMissing = new(
        "UserRequiredDataMissing",
        ErrorType.Validation,
        "Required data missing from create user request."
    );
    
    public static Error InsufficientFunds = new(
        "InsufficientFunds",
        ErrorType.Validation,
        "Insufficient balance."
    );
}


