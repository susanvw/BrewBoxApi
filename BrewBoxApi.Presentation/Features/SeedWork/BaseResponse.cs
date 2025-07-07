namespace BrewBoxApi.Presentation.Features.SeedWork;

/// <summary>
/// Represents the base response for an operation, indicating success or failure and an optional error message.
/// </summary>
public sealed class BaseResponse<T>
{
    public T? Result { get; }
    public BaseResponse(T result)
    {
        Success = true;
        Result = result;
    }
    public BaseResponse(IEnumerable<string> errors)
    {
        Success = false;
        Errors = errors;
    }

    public IEnumerable<string>? Errors { get; }
    public bool Success { get; }

    internal static BaseResponse<T> Succeeded(T result) => new(result);
    internal static BaseResponse<T> Failed(IEnumerable<string> errors) => new(errors);
}
