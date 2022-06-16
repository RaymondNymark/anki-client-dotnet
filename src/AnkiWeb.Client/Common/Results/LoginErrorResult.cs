namespace AnkiWeb.Client.Common.Results;

internal class LoginErrorResult : ErrorResult
{
    public LoginErrorResult(string message) : base(message)
    {
    }

    public LoginErrorResult(string message, IReadOnlyCollection<Error> errors) : base(message, errors)
    {
    }
}

