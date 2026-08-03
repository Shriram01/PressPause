namespace PressPause.Application.Common.Models;

public sealed class Result
{
    private Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToList().AsReadOnly();
    }

    public bool Succeeded { get; }

    public IReadOnlyList<string> Errors { get; }

    public static Result Success()
    {
        return new Result(true, []);
    }

    public static Result Failure(params string[] errors)
    {
        return new Result(false, errors);
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }
}