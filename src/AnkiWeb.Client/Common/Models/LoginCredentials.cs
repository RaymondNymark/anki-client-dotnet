namespace AnkiWeb.Client.Common.Models;

public record LoginCredentials
{
    public LoginCredentials(string username, string password)
    {
        Username = username;
        Password = password;
    }

    public string Username { get; set; }
    public string Password { get; set; }
}

