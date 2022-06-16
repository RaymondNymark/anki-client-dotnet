namespace AnkiWeb.Client.Common.Results;

internal class CsrfErrorResult : ErrorResult
{
    public CsrfErrorResult(string message) : base(message)
    {
    }

    public CsrfErrorResult(string message, IReadOnlyCollection<Error> errors) : base(message, errors)
    {
    }
}

