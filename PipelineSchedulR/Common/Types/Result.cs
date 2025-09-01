namespace PipelineSchedulR.Common.Types;

public readonly struct Result
{
    private readonly string? _error;
    private Result(string? error)
    {
        IsFailure = error != null;
        _error = error;
    }
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;
    public string Error => IsFailure ? _error! : throw new InvalidOperationException("Unable to access error for successful result");

    public static Result Success() => new(null);
    public static Result Failure(string error) => new(error);
}
