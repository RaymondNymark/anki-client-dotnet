namespace AnkiWeb.Client.Common.Results;

internal class InvalidCardResult : ErrorResult
{
    public InvalidCardResult(string message) : base(message)
    {
    }

    public InvalidCardResult(string message, IReadOnlyCollection<Error> errors) : base(message, errors)
    {
    }
}
