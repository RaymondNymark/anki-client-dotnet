namespace AnkiWeb.Client.Helpers;

public record AnkiClientStatus
{
    public AnkiClientStatus(bool success, string? message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
}

