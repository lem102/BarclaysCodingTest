using BarclaysCodingTest.Dtos;

namespace BarclaysCodingTest.Utilities;

public record Result
{
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
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
    public static Error UsernameUnavailable = new(
        "UsernameUnavailable",
        ErrorType.Validation,
        "User name is unavailable"
    );

    public static Error UserNotFound(Guid id) => new(
        "UserNotFound",
        ErrorType.NotFound,
        $"User with id \"{id.ToString()}\" not found"
    );

    public static Error IncorrectLoginDetails() => new(
        "IncorrectLoginDetails",
        ErrorType.Validation,
        "Login details provided are incorrect"
    );

    internal static Error UserUnauthorized(Guid id)
        => new("UserUnauthorized", ErrorType.Unauthorized, $"User with id '{id}' is unauthorized");
    
}


