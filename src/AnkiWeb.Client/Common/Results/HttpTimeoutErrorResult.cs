namespace AnkiWeb.Client.Common.Results;

internal class HttpTimeoutErrorResult : ErrorResult
{
    public HttpTimeoutErrorResult(string message) : base(message)
    {
    }

    public HttpTimeoutErrorResult(string message, IReadOnlyCollection<Error> errors) : base(message, errors)
    {
    }
}

